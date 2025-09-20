using System.Windows.Threading;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.ErrorManagement
{
    public sealed class UncaughtExceptionService(IUserNotifier notifier) : IHostedService
    {
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            System.Windows.Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            System.Windows.Application.Current.DispatcherUnhandledException -= OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            return Task.CompletedTask;
        }

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                notifier.NotifyError(
                    nameof(App),
                    ex
                );
            else
                notifier.NotifyError(
                    nameof(App),
                    "An unknown error occurred."
                );
        }

        private void OnDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            notifier.NotifyError(
                nameof(App),
                e.Exception
            );
            e.Handled = true;
        }

        private void OnUnobservedTaskException(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            notifier.NotifyError(
                nameof(App),
                e.Exception
            );
            e.SetObserved();
        }
    }
}