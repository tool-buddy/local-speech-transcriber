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
    /// <summary>
    /// DI registration helpers for the application, infrastructure, and presentation layers.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the application layer services and binds configuration options.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The application configuration root.</param>
        /// <returns>The same service collection for chaining.</returns>
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

        /// <summary>
        /// Registers the infrastructure layer implementations (audio, Python locator, transcription engine, keyboard).
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection AddInfrastructureLayer(
            this IServiceCollection services)
        {
            services.AddSingleton<IAudioRecorder, NAudioRecorder>();
            services.AddSingleton<IPythonLocator, CrossPlatformPythonLocator>();
            services.AddSingleton<ITranscriptionEngine, WhisperEngine>();
            services.AddSingleton<IKeyboardOutput, InputSimulatorKeyboardOutput>();

            return services;
        }

        /// <summary>
        /// Registers the presentation layer services, hosted services, view models, and main window.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The same service collection for chaining.</returns>
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