namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    //todo document
    public interface ISttEngine
    {
        event EventHandler<string>? Transcribed;
        event EventHandler? Initialized;

        void Initialize();

        Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded);
    }
}