using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Settings;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed partial class WhisperStreamingSttEngine : ISttEngine, IDisposable
    {
        //todo review class
        private readonly IErrorDisplayer _errorDisplayer;
        private readonly Client _client;
        private readonly Server _server;

        public event EventHandler<string>? Transcribed;
        public event EventHandler? Initialized;

        public WhisperStreamingSttEngine(
            IErrorDisplayer errorDisplayer,
            IOptions<WhisperSettings> whisperSettings)
        {
            _errorDisplayer = errorDisplayer;

            _server = new Server(
                whisperSettings.Value.Port,
                whisperSettings.Value.Model,
                errorDisplayer
            );

            _server.Started += OnStared;

            _client = new Client();
        }

        public void Initialize() =>
            _server.Start();

        public async Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded) =>
            await _client.SendAudioDataAsync(
                buffer,
                bytesRecorded
            ).ConfigureAwait(false);

        public void Dispose()
        {
            _client.TranscriptionReceived -= OnClientTranscriptionReceived;
            _client.Dispose();

            _server.Started -= OnStared;
            _server.Dispose();
        }


        private async void OnStared(
            object? sender,
            EventArgs args)
        {
            try
            {
                //todo handle async warning
                await _client.ConnectAsync(
                    "localhost",
                    _server.Port
                ).ConfigureAwait(false);

                _client.TranscriptionReceived += OnClientTranscriptionReceived;

                _client.StartListening();

                Initialized?.Invoke(
                    this,
                    EventArgs.Empty
                );
            }
            catch (Exception e)
            {
                _errorDisplayer.Exception(
                    nameof(WhisperStreamingSttEngine),
                    e
                );
            }
        }

        private void OnClientTranscriptionReceived(
            object? _,
            string text) =>
            Transcribed?.Invoke(
                this,
                text
            );
    }
}