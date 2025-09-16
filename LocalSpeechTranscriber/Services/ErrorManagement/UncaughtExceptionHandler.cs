using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Hosting;

namespace ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement
{
    public sealed class UncaughtExceptionHandler : IDisposable
    {
        private readonly IHost _host;

        public UncaughtExceptionHandler(
            IHost host)
        {
            _host = host;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }


        public void Dispose()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            Application.Current.DispatcherUnhandledException -= OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            if (_host.Services.GetService(typeof(IErrorDisplayer)) is not IErrorDisplayer errorDisplayer)
                return;

            if (e.ExceptionObject is Exception ex)
                errorDisplayer.Exception(
                    nameof(App),
                    ex
                );
            else
                errorDisplayer.Error(
                    nameof(App),
                    "An unknown error occurred."
                );
        }

        private void OnDispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            if (_host.Services.GetService(typeof(IErrorDisplayer)) is not IErrorDisplayer errorDisplayer)
                return;

            errorDisplayer.Exception(
                nameof(App),
                e.Exception
            );
            e.Handled = true;
        }

        private void OnUnobservedTaskException(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            if (_host.Services.GetService(typeof(IErrorDisplayer)) is not IErrorDisplayer errorDisplayer)
                return;

            errorDisplayer.Exception(
                nameof(App),
                e.Exception
            );
            e.SetObserved();
        }
    }
}