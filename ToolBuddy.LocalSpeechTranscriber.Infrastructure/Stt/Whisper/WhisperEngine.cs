using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper
{
    public sealed class WhisperEngine : ITranscriptionEngine, IDisposable
    {
        private readonly WhisperClient _client;
        private readonly IUserNotifier _notifier;
        private readonly WhisperServerProcess _server;

        public WhisperEngine(
            IOptions<WhisperSettings> whisperOptions,
            IUserNotifier notifier,
            ILogger<WhisperEngine> logger)
        {
            _notifier = notifier;

            WhisperSettings settings = whisperOptions.Value;
            _server = new WhisperServerProcess(
                settings.Port,
                settings.Model,
                settings.PythonExecutable,
                logger
            );
            _server.Started += OnServerStarted;

            _client = new WhisperClient(notifier);
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
                _notifier.Exception(
                    nameof(WhisperEngine),
                    e
                );
            }
        }

        private void OnTranscriptionReceived(
            object? sender,
            string text)
            => Transcribed?.Invoke(
                this,
                text
            );
    }
}