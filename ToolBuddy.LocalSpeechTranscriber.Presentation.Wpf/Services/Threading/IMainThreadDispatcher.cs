namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Threading
{
    /// <summary>
    /// Framework-agnostic dispatcher abstraction to marshal actions onto the UI/main thread.
    /// </summary>
    public interface IMainThreadDispatcher
    {
        /// <summary>
        /// Posts an action to be executed on the UI/main thread asynchronously.
        /// </summary>
        /// <param name="action">The action to invoke on the main thread.</param>
        void Post(
            Action action);
    }
}