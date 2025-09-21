using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper
{
    /// <summary>
    /// Transcription engine that manages a local Whisper server process and a client connection
    /// to stream audio and receive transcription results.
    /// </summary>
    public sealed class WhisperEngine : ITranscriptionEngine, IDisposable
    {
        private readonly IUserNotifier _notifier;
        private readonly ILogger<WhisperEngine> _logger;
        private readonly WhisperClient _client;
        private readonly BaseWhisperServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhisperEngine"/> class.
        /// </summary>
        /// <param name="whisperOptions">Whisper configuration options.</param>
        /// <param name="pythonLocator">Locator used to find a Python interpreter.</param>
        /// <param name="notifier">User notification service for surfacing errors.</param>
        /// <param name="logger">Logger instance.</param>
        public WhisperEngine(
            IOptions<WhisperSettings> whisperOptions,
            IPythonLocator pythonLocator,
            IUserNotifier notifier,
            ILogger<WhisperEngine> logger)
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
        /// <param name="settings">The Whisper server settings.</param>
        /// <param name="pythonLocator">Python locator for server scripts.</param>
        /// <param name="logger">Logger.</param>
        /// <returns>A configured <see cref="BaseWhisperServer"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown implementation is specified.</exception>
        private static BaseWhisperServer CreateServer(
            WhisperSettings settings,
            IPythonLocator pythonLocator,
            ILogger<WhisperEngine> logger)
        {
            BaseWhisperServer result = settings.Implementation switch
            {
                WhisperImplementation.WhisperStreaming => new WhisperStreamingServer(
                    settings.Port,
                    settings.Model,
                    pythonLocator,
                    logger
                ),
                WhisperImplementation.SimulStreaming => new SimulStreamingServer(
                    settings.Port,
                    settings.Model,
                    pythonLocator,
                    logger
                ),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(settings),
                    $"Unknown value of {nameof(WhisperSettings.Implementation)}: {settings.Implementation}"
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
                    nameof(WhisperEngine),
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