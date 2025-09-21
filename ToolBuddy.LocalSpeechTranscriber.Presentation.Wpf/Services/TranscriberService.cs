using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services
{
    /// <summary>
    /// Hosted service that initializes the <see cref="Transcriber"/> at application startup.
    /// </summary>
    /// <param name="transcriber">The application transcriber orchestrating recording and STT.</param>
    public sealed class TranscriberService(Transcriber transcriber) : IHostedService
    {
        /// <inheritdoc />
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialize();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(
            CancellationToken cancellationToken) => Task.CompletedTask;
    }
}