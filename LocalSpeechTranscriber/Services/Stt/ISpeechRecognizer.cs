namespace ToolBuddy.LocalSpeechTranscriber.Services.Stt
{
    //todo document
    public interface ISpeechRecognizer
    {
        event EventHandler<string> TranscriptionDone;
        event EventHandler Initialized;

        void Initialize();

        Task Transcribe(
            byte[] buffer,
            int bytesRecorded);
    }
}