using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services
{
    public sealed class TranscriberService(Transcriber transcriber) : IHostedService
    {
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialize();
            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken) => Task.CompletedTask;
    }
}