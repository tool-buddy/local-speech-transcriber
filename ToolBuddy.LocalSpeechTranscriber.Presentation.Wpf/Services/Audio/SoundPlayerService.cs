using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Audio
{
    public sealed class SoundPlayerService(Transcriber transcriber, IUserNotifier notifier) : IHostedService
    {
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialized += OnTranscriberInitialized;
            transcriber.RecordingStarted += OnRecordingStarted;
            transcriber.RecordingStopped += OnRecordingStopped;
            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialized -= OnTranscriberInitialized;
            transcriber.RecordingStarted -= OnRecordingStarted;
            transcriber.RecordingStopped -= OnRecordingStopped;
            return Task.CompletedTask;
        }

        private void BeepAsync(
            int frequency,
            int duration) =>
            Task.Run(() =>
                {
                    try
                    {
                        Console.Beep(
                            frequency,
                            duration
                        );
                    }
                    catch (Exception e)
                    {
                        notifier.NotifyError(
                            nameof(SoundPlayerService),
                            e
                        );
                    }
                }
            );

        private void OnRecordingStopped(
            object? sender,
            EventArgs e) => BeepAsync(
            659,
            200
        );

        private void OnRecordingStarted(
            object? sender,
            EventArgs e) => BeepAsync(
            330,
            200
        );

        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) => BeepAsync(
            494,
            100
        );
    }
}