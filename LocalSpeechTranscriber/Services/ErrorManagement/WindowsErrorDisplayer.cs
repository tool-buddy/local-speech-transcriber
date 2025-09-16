using System.Windows;

namespace ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement
{
    public sealed class WindowsErrorDisplayer : IErrorDisplayer
    {
        public void Error(
            string errorSource,
            string errorMessage) =>
            MessageBox.Show(
                $"{errorSource}: {errorMessage}",
                //todo name of current app
                "LocalSpeechTranscriber",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

        public void Exception(
            string errorSource,
            Exception exception) =>
            Error(
                errorSource,
                $"{exception.Message}\n{exception.StackTrace}"
            );
    }
}