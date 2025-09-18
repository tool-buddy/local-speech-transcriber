using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.ErrorManagement
{
    public sealed class WindowsUserNotifier(IAppInfo appInfo) : IUserNotifier
    {
        public void Error(
            string errorSource,
            string errorMessage) =>
            MessageBox.Show(
                $"{errorSource}: {errorMessage}",
                appInfo.ProductName,
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