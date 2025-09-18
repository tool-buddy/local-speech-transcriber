using Microsoft.Extensions.Options;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Settings;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Stt
{
    public sealed partial class WhisperStreamingSttEngine : ISttEngine, IDisposable
    {
        private readonly Client _client;
        private readonly IErrorDisplayer _errorDisplayer;
        private readonly Server _server;

        public WhisperStreamingSttEngine(
            IErrorDisplayer errorDisplayer,
            IOptions<WhisperSettings> whisperSettings)
        {
            _errorDisplayer = errorDisplayer;

            _server = new Server(
                whisperSettings.Value.Port,
                whisperSettings.Value.Model,
                whisperSettings.Value.PythonExecutable
            );

            _server.Started += OnServerStarted;

            _client = new Client(errorDisplayer);
        }

        public void Dispose()
        {
            _client.TranscriptionReceived -= OnClientTranscriptionReceived;
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
            int bytesRecorded) =>
            await _client.SendAudioDataAsync(
                buffer,
                bytesRecorded
            ).ConfigureAwait(false);


        private async void OnServerStarted(
            object? sender,
            EventArgs args)
        {
            try
            {
                _client.TranscriptionReceived += OnClientTranscriptionReceived;

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
