using System.Windows;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.ErrorManagement
{
    /// <summary>
    /// Windows-specific implementation of <see cref="IUserNotifier"/> that displays error messages
    /// using a modal <see cref="MessageBox"/>.
    /// </summary>
    /// <param name="appInfo">Application information provider, used to display the application name on the messagebox title.</param>
    public sealed class WindowsUserNotifier(IAppInfo appInfo) : IUserNotifier
    {
        /// <inheritdoc />
        public void NotifyError(
            string source,
            string message) =>
            MessageBox.Show(
                $"{source}: {message}",
                appInfo.ProductName,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

        /// <inheritdoc />
        public void NotifyError(
            string source,
            Exception exception) =>
            NotifyError(
                source,
                $"{exception.Message}\n{exception.StackTrace}"
            );
    }
}