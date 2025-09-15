using System.Windows;
using System.Windows.Threading;
using GregsStack.InputSimulatorStandard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Services;
using ToolBuddy.LocalSpeechTranscriber.Settings;
using ToolBuddy.LocalSpeechTranscriber.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber
{
    public partial class App : Application
    {
        private IHost _host = null!;

        protected override void OnStartup(
            StartupEventArgs e)
        {
            _host = GetHost();

            HandleUnhandledExceptions();

            _host.Start();

            _host.Services.GetRequiredService<Transcriber>().Initialize();
            _host.Services.GetRequiredService<RecordingHotkeyToggler>().Initialize();
            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(
            ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

        private void HandleUnhandledExceptions()
        {
            //todo is this different from what WPF does by default?
            //todo move in a dedicated class?
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static IHost GetHost() =>
            Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((
                        ctx,
                        cfg) =>
                    {
                        cfg.SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile(
                                "appsettings.json",
                                false,
                                true
                            );
                    }
                )
                .ConfigureServices((
                        ctx,
                        services) =>
                    {
                        services.AddOptions<WhisperSettings>()
                            .Bind(ctx.Configuration.GetSection(WhisperSettings.SectionName))
                            .ValidateDataAnnotations()
                            .ValidateOnStart();

                        services.AddOptions<HotkeysSettings>()
                            .Bind(ctx.Configuration.GetSection(HotkeysSettings.SectionName))
                            .ValidateDataAnnotations()
                            .ValidateOnStart();

                        services.AddSingleton<IErrorDisplayer, WindowsErrorDisplayer>();
                        services.AddSingleton<AudioRecorder>();
                        services.AddSingleton<ISttEngine, WhisperStreamingSttEngine>();
                        services.AddSingleton<InputSimulator>();
                        services.AddSingleton<Transcriber>();
                        services.AddSingleton<SoundPlayer>();
                        services.AddSingleton<MainViewModel>();

                        services.AddSingleton<MainWindow>();
                    }
                )
                .Build();

        #region Error handling

        private void OnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs e)
        {
            IErrorDisplayer errorDisplayer = _host.Services.GetRequiredService<IErrorDisplayer>();
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
            IErrorDisplayer errorDisplayer = _host.Services.GetRequiredService<IErrorDisplayer>();
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
            IErrorDisplayer errorDisplayer = _host.Services.GetRequiredService<IErrorDisplayer>();
            errorDisplayer.Exception(
                nameof(App),
                e.Exception
            );
            e.SetObserved();
        }

        #endregion
    }
}