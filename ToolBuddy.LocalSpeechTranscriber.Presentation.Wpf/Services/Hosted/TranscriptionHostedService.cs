using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Orchestration;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Hosted
{
    /// <summary>
    /// Hosted service that initializes the <see cref="TranscriptionOrchestrator"/> at application startup.
    /// </summary>
    /// <param name="transcriptionOrchestrator">The transcriber orchestrating recording and STT.</param>
    public sealed class TranscriptionHostedService(TranscriptionOrchestrator transcriptionOrchestrator) : IHostedService
    {
        /// <inheritdoc />
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            transcriptionOrchestrator.Initialize();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(
            CancellationToken cancellationToken) => Task.CompletedTask;
    }
}