using System.Windows;
using GregsStack.InputSimulatorStandard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Services;
using ToolBuddy.LocalSpeechTranscriber.Services.Audio;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Services.Hotkeys;
using ToolBuddy.LocalSpeechTranscriber.Services.Stt;
using ToolBuddy.LocalSpeechTranscriber.Settings;
using ToolBuddy.LocalSpeechTranscriber.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber
{
    public partial class App : Application
    {
        private IHost _host = null!;
        private UncaughtExceptionHandler _uncaughtExceptionHandler = null!;

        protected override void OnStartup(
            StartupEventArgs e)
        {
            _host = GetHost();

            _uncaughtExceptionHandler = new UncaughtExceptionHandler(_host);

            Start();
        }

        private void Start()
        {
            _host.Start();

            _host.Services.GetRequiredService<Transcriber>().Initialize();
            _host.Services.GetRequiredService<RecordingHotkeyToggler>().Initialize();
            _ = _host.Services.GetRequiredService<SoundPlayer>();
            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(
            ExitEventArgs e)
        {
            _uncaughtExceptionHandler.Dispose();
            _host.Dispose();
            base.OnExit(e);
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
                            )
                            .AddJsonFile(
                                "appsettings.Local.json",
                                true,
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
                        services.AddSingleton<RecordingHotkeyToggler>();
                        services.AddSingleton<SoundPlayer>();
                        services.AddSingleton<MainViewModel>();

                        services.AddSingleton<MainWindow>();
                    }
                )
                .Build();
    }
}