using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Orchestration;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Audio;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Input;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Python;
using ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Abstractions;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Adapters;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Hosted;
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
            services.AddOptions<WhisperOptions>()
                .Bind(configuration.GetSection(WhisperOptions.SectionName))
                .ValidateDataAnnotations();
            services.AddOptions<HotkeysOptions>()
                .Bind(configuration.GetSection(HotkeysOptions.SectionName))
                .ValidateDataAnnotations();

            services.AddSingleton<TranscriptionOrchestrator>();

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
            services.AddSingleton<ITranscriber, WhisperTranscriber>();
            services.AddSingleton<IKeyboardTyper, InputSimulatorKeyboardTyper>();

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
            services.AddHostedService<UncaughtExceptionHandlingHostedService>();
            services.AddHostedService<TranscriptionHostedService>();
            services.AddHostedService<HotkeyRegistrationHostedService>();
            services.AddHostedService<SoundPlayingHostedService>();

            services.AddSingleton<IAppInfoProvider, AppInfoProvider>();
            services.AddSingleton<IUserNotifier, WindowsUserNotifier>();
            services.AddSingleton<IUIDispatcher, WpfUIDispatcher>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            return services;
        }
    }
}