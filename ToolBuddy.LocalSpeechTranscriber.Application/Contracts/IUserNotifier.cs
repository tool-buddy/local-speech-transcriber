namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    public interface IUserNotifier
    {
        void NotifyError(
            string source,
            string message);

        void NotifyError(
            string source,
            Exception exception);
    }
}