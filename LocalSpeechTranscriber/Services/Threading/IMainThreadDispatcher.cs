namespace ToolBuddy.LocalSpeechTranscriber.Services.Threading
{
    /// <summary>
    /// Framework-agnostic dispatcher abstraction to marshal actions onto the UI/main thread.
    /// </summary>
    public interface IMainThreadDispatcher
    {
        void Post(Action action);
    }
}
