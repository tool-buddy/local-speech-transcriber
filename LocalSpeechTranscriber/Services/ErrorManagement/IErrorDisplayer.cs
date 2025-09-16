namespace ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement
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