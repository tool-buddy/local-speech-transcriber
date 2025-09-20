namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    /// <summary>
    /// Provides user-facing notifications for errors and important events.
    /// </summary>
    public interface IUserNotifier
    {
        /// <summary>
        /// Notifies the user about an error with a message.
        /// </summary>
        /// <param name="source">A short source identifier (e.g., component or feature name).</param>
        /// <param name="message">The human-readable error message.</param>
        void NotifyError(
            string source,
            string message);

        /// <summary>
        /// Notifies the user about an error with exception details.
        /// </summary>
        /// <param name="source">A short source identifier (e.g., component or feature name).</param>
        /// <param name="exception">The exception that describes the error.</param>
        void NotifyError(
            string source,
            Exception exception);
    }
}