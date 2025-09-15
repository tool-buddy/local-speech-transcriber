namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    //todo namespaces for all
    public sealed class SoundPlayer: IDisposable
    {
        private readonly Transcriber _transcriber;

        public SoundPlayer(
            Transcriber transcriber)
        {
            _transcriber = transcriber;
            _transcriber.RecordingStarted += OnRecordingStarted();
            _transcriber.RecordingStopped += OnRecordingStopped();
        }

        private static EventHandler? OnRecordingStopped() =>
        (
            _,
            _) => Console.Beep(
            659,
            200
        );

        private static EventHandler? OnRecordingStarted() =>
        (
            _,
            _) => Console.Beep(
            330,
            200
        );

        public void Dispose()
        {
            _transcriber.RecordingStarted -= OnRecordingStarted();
            _transcriber.RecordingStopped -= OnRecordingStopped();
        }
    }
}