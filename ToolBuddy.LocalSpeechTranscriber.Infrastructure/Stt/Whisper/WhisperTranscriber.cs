using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Options;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper
{
    /// <summary>
    /// Transcription engine that manages a local Whisper server process and a client connection
    /// to stream audio and receive transcription results.
    /// </summary>
    public sealed class WhisperTranscriber : ITranscriber, IDisposable
    {
        private readonly IUserNotifier _notifier;
        private readonly ILogger<WhisperTranscriber> _logger;
        private readonly WhisperClient _client;
        private readonly WhisperServerBase _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhisperTranscriber"/> class.
        /// </summary>
        /// <param name="whisperOptions">Whisper configuration options.</param>
        /// <param name="pythonLocator">Locator used to find a Python interpreter.</param>
        /// <param name="notifier">User notification service for surfacing errors.</param>
        /// <param name="logger">Logger instance.</param>
        public WhisperTranscriber(
            IOptions<WhisperOptions> whisperOptions,
            IPythonLocator pythonLocator,
            IUserNotifier notifier,
            ILogger<WhisperTranscriber> logger)
        {
            _notifier = notifier;
            _logger = logger;

            _server = CreateServer(
                whisperOptions.Value,
                pythonLocator,
                logger
            );
            _server.Started += OnServerStarted;

            _client = new WhisperClient(
                notifier,
                logger
            );
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client.TranscriptionReceived -= OnTranscriptionReceived;
            _client.Dispose();

            _server.Started -= OnServerStarted;
            _server.Dispose();
        }

        /// <inheritdoc />
        public event EventHandler<string>? Transcribed;

        /// <inheritdoc />
        public event EventHandler? Initialized;

        /// <inheritdoc />
        public void Initialize() =>
            _server.Start();

        /// <inheritdoc />
        public async Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded)
            => await _client.SendAudioDataAsync(
                buffer,
                bytesRecorded
            ).ConfigureAwait(false);

        /// <summary>
        /// Creates the appropriate Whisper server implementation based on settings.
        /// </summary>
        /// <param name="options">The Whisper server settings.</param>
        /// <param name="pythonLocator">Python locator for server scripts.</param>
        /// <param name="logger">Logger.</param>
        /// <returns>A configured <see cref="WhisperServerBase"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown implementation is specified.</exception>
        private static WhisperServerBase CreateServer(
            WhisperOptions options,
            IPythonLocator pythonLocator,
            ILogger<WhisperTranscriber> logger)
        {
            WhisperServerBase result = options.ImplementationKind switch
            {
                WhisperImplementationKind.WhisperStreaming => new WhisperStreamingServer(
                    options.Port,
                    options.Model,
                    pythonLocator,
                    logger
                ),
                WhisperImplementationKind.SimulStreaming => new SimulStreamingServer(
                    options.Port,
                    options.Model,
                    pythonLocator,
                    logger
                ),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(options),
                    $"Unknown value of {nameof(WhisperOptions.ImplementationKind)}: {options.ImplementationKind}"
                )
            };

            return result;
        }

        /// <summary>
        /// Handles the server Started event, connects the client, and signals initialization.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        private async void OnServerStarted(
            object? sender,
            EventArgs args)
        {
            try
            {
                _client.TranscriptionReceived += OnTranscriptionReceived;

                await _client.ConnectAsync(
                    "localhost",
                    _server.Port
                ).ConfigureAwait(false);

                Initialized?.Invoke(
                    this,
                    EventArgs.Empty
                );
            }
            catch (Exception e)
            {
                _notifier.NotifyError(
                    nameof(WhisperTranscriber),
                    e
                );
            }
        }

        /// <summary>
        /// Handles transcription results from the client and forwards them to subscribers.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="text">The text that was transcribed.</param>
        private void OnTranscriptionReceived(
            object? sender,
            string text)
        {
            _logger.LogInformation(
                "{Text}",
                text
            );

            Transcribed?.Invoke(
                this,
                text
            );
        }
    }
}