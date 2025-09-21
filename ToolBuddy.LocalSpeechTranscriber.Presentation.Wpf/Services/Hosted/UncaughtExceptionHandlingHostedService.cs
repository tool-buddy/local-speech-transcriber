using System.Windows.Threading;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Hosted
{
    /// <summary>
    /// Hosted service that centralizes handling of uncaught exceptions, and reports them via <see cref="IUserNotifier"/>.
    /// </summary>
    /// <param name="notifier">The user notifier used to surface error information.</param>
    public sealed class UncaughtExceptionHandlingHostedService(IUserNotifier notifier) : IHostedService
    {
        /// <inheritdoc />
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            System.Windows.Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            System.Windows.Application.Current.DispatcherUnhandledException -= OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles unhandled AppDomain exceptions.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The unhandled exception event arguments.</param>
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

        /// <summary>
        /// Handles application exceptions.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The dispatcher unhandled exception event arguments.</param>
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

        /// <summary>
        /// Handles unobserved task exceptions.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The unobserved task exception event arguments.</param>
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