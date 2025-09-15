namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    //todo rename all
    public interface IErrorDisplayer
    {
        void Error(
            string errorMessage,
            string errorSource);

        void Exception(
            string errorSource,
            Exception exception);
    }
}