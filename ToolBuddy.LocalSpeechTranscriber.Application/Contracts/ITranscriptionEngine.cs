namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    public interface ITranscriptionEngine
    {
        event EventHandler<string>? Transcribed;
        event EventHandler? Initialized;

        void Initialize();

        Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded);
    }
}