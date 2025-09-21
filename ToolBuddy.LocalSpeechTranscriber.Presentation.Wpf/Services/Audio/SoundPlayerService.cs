using Microsoft.Extensions.Hosting;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Audio
{
    /// <summary>
    /// Plays short beep sounds to signal important application events.
    /// </summary>
    /// <param name="transcriber">The application transcriber to subscribe to for lifecycle events.</param>
    /// <param name="notifier">User notifier used to surface any playback errors.</param>
    public sealed class SoundPlayerService(Transcriber transcriber, IUserNotifier notifier) : IHostedService
    {

        /// <inheritdoc />
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialized += OnTranscriberInitialized;
            transcriber.RecordingStarted += OnRecordingStarted;
            transcriber.RecordingStopped += OnRecordingStopped;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            transcriber.Initialized -= OnTranscriberInitialized;
            transcriber.RecordingStarted -= OnRecordingStarted;
            transcriber.RecordingStopped -= OnRecordingStopped;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Asynchronously plays a beep tone.
        /// </summary>
        /// <param name="frequency">The beep frequency in hertz.</param>
        /// <param name="duration">The beep duration in milliseconds.</param>
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

        /// <summary>
        /// Plays the "recording stopped" tone.
        /// </summary>
        private void OnRecordingStopped(
            object? sender,
            EventArgs e) => BeepAsync(
            659,
            200
        );

        /// <summary>
        /// Plays the "recording started" tone.
        /// </summary>
        private void OnRecordingStarted(
            object? sender,
            EventArgs e) => BeepAsync(
            330,
            200
        );

        /// <summary>
        /// Plays the "initialized" tone.
        /// </summary>
        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) => BeepAsync(
            494,
            100
        );
    }
}