namespace ToolBuddy.LocalSpeechTranscriber.Services.Stt
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
