using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.ErrorManagement
{
    public sealed class WindowsUserNotifier(IAppInfo appInfo) : IUserNotifier
    {
        public void NotifyError(
            string source,
            string message) =>
            MessageBox.Show(
                $"{source}: {message}",
                appInfo.ProductName,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

        public void NotifyError(
            string source,
            Exception exception) =>
            NotifyError(
                source,
                $"{exception.Message}\n{exception.StackTrace}"
            );
    }
}