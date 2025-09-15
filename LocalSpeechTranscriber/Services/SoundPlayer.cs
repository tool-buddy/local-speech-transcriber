namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    //todo namespaces for all
    public class SoundPlayer
    {
        public SoundPlayer(
            Transcriber transcriber)
        {
            // todo is this disposing compatible?
            transcriber.RecordingStarted += (
                _,
                _) => Console.Beep(
                330,
                200
            );

            transcriber.RecordingStopped += (
                _,
                _) => Console.Beep(
                659,
                200
            );
        }
    }
}