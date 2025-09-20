using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper
{
    public sealed class WhisperEngine : ITranscriptionEngine, IDisposable
    {
        private readonly IUserNotifier _notifier;
        private readonly ILogger<WhisperEngine> _logger;
        private readonly WhisperClient _client;
        private readonly BaseWhisperServer _server;

        public WhisperEngine(
            IOptions<WhisperSettings> whisperOptions,
            IUserNotifier notifier,
            ILogger<WhisperEngine> logger)
        {
            _notifier = notifier;
            _logger = logger;

            _server = CreateServer(
                whisperOptions.Value,
                logger
            );
            _server.Started += OnServerStarted;

            _client = new WhisperClient(
                notifier,
                logger
            );
        }

        public void Dispose()
        {
            _client.TranscriptionReceived -= OnTranscriptionReceived;
            _client.Dispose();

            _server.Started -= OnServerStarted;
            _server.Dispose();
        }

        public event EventHandler<string>? Transcribed;
        public event EventHandler? Initialized;

        public void Initialize() =>
            _server.Start();

        public async Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded)
            => await _client.SendAudioDataAsync(
                buffer,
                bytesRecorded
            ).ConfigureAwait(false);

        private static BaseWhisperServer CreateServer(
            WhisperSettings settings,
            ILogger<WhisperEngine> logger)
        {
            BaseWhisperServer result;
            switch (settings.Implementation)
            {
                case WhisperImplementation.WhisperStreaming:
                    result = new WhisperStreamingServer(
                        settings.Port,
                        settings.Model,
                        settings.PythonExecutable,
                        logger
                    );
                    break;
                case WhisperImplementation.SimulStreaming:
                    result = new SimulStreamingServer(
                        settings.Port,
                        settings.Model,
                        settings.PythonExecutable,
                        logger
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

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