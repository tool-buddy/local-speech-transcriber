using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.CompositionRoot;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf
{
    /// <summary>
    /// WPF application entry point.
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost _host = null!;

        /// <summary>
        /// Handles application startup by creating and starting the host, then showing the main window.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override void OnStartup(
            StartupEventArgs e)
        {
            _host = GetHost();
            _host.Start();
            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        /// <summary>
        /// Handles application exit.
        /// </summary>
        /// <param name="e">Exit event arguments.</param>
        protected override void OnExit(
            ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// Configures and returns an <see cref="IHost"/> with application, infrastructure, and presentation layers.
        /// </summary>
        /// <returns>The configured application host.</returns>
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
                        services.AddApplicationLayer(ctx.Configuration);
                        services.AddInfrastructureLayer();
                        services.AddPresentationLayer();
                    }
                )
                .Build();
    }
}