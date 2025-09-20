using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Audio;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Input;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Python;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Audio;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Hotkeys;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Threading;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.CompositionRoot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<WhisperSettings>()
                .Bind(configuration.GetSection(WhisperSettings.SectionName))
                .ValidateDataAnnotations();
            services.AddOptions<HotkeysSettings>()
                .Bind(configuration.GetSection(HotkeysSettings.SectionName))
                .ValidateDataAnnotations();

            services.AddSingleton<Transcriber>();

            return services;
        }

        public static IServiceCollection AddInfrastructureLayer(
            this IServiceCollection services,
            IConfiguration _)
        {
            services.AddSingleton<IAudioRecorder, NAudioRecorder>();
            services.AddSingleton<IPythonLocator, CrossPlatformPythonLocator>();
            services.AddSingleton<ITranscriptionEngine, WhisperEngine>();
            services.AddSingleton<IKeyboardOutput, InputSimulatorKeyboardOutput>();

            return services;
        }

        public static IServiceCollection AddPresentationLayer(
            this IServiceCollection services)
        {
            services.AddHostedService<TranscriberService>();
            services.AddHostedService<RecordingHotkeyService>();
            services.AddHostedService<UncaughtExceptionService>();
            services.AddHostedService<SoundPlayerService>();

            services.AddSingleton<IAppInfo, AppInfo>();
            services.AddSingleton<IUserNotifier, WindowsUserNotifier>();
            services.AddSingleton<IMainThreadDispatcher, WpfMainThreadDispatcher>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            return services;
        }
    }
}