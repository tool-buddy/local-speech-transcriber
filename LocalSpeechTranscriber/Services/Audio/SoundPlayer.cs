namespace ToolBuddy.LocalSpeechTranscriber.Services.Audio
{
    //todo namespaces for all
    public sealed class SoundPlayer : IDisposable
    {
        private readonly Transcriber _transcriber;

        public SoundPlayer(
            Transcriber transcriber)
        {
            _transcriber = transcriber;
            _transcriber.Initialized += OnTranscriberInitialized;
            _transcriber.RecordingStarted += OnRecordingStarted;
            _transcriber.RecordingStopped += OnRecordingStopped;
        }

        public void Dispose()
        {
            _transcriber.Initialized -= OnTranscriberInitialized;
            _transcriber.RecordingStarted -= OnRecordingStarted;
            _transcriber.RecordingStopped -= OnRecordingStopped;
        }

        private void OnRecordingStopped(
            object? sender,
            EventArgs e) =>
            Console.Beep(
                659,
                200
            );

        private void OnRecordingStarted(
            object? sender,
            EventArgs e) =>
            Console.Beep(
                330,
                200
            );

        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) =>
            Console.Beep(
                494,
                100
            );
    }
}