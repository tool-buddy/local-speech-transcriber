using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.CompositionRoot;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf
{
    public partial class App
    {
        private IHost _host = null!;

        protected override void OnStartup(
            StartupEventArgs e)
        {
            _host = GetHost();
            Start();
        }

        private void Start()
        {
            _host.Start();
            _host.Services.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(
            ExitEventArgs e)
        {
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
                        services.AddApplicationLayer(ctx.Configuration);
                        services.AddInfrastructureLayer(ctx.Configuration);
                        services.AddPresentationLayer();
                    }
                )
                .Build();
    }
}