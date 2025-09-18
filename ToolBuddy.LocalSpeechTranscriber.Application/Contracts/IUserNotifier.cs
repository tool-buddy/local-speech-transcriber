namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    public interface IUserNotifier
    {
        void Error(
            string errorSource,
            string errorMessage);

        void Exception(
            string errorSource,
            Exception exception);
    }
}