using System.Windows.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NHotkey;
using NHotkey.Wpf;
using ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Hotkeys
{
    /// <summary>
    /// Registers and handles the global hotkey that toggles audio recording.
    /// </summary>
    /// <param name="hotkeysSettings">Provides configured hotkey options.</param>
    /// <param name="transcriber">The application transcriber to control.</param>
    /// <param name="userNotifier">User notifier to surface registration errors.</param>
    public sealed class RecordingHotkeyService(
        IOptions<HotkeysSettings> hotkeysSettings,
        Transcriber transcriber,
        IUserNotifier userNotifier) : IHostedService
    {
        private const string ToggleRecordingHotkeyName = "ToggleRecording";

        /// <inheritdoc />
        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                SetupHotkey();
            }
            catch (HotkeyAlreadyRegisteredException e)
            {
                userNotifier.NotifyError(
                    nameof(RecordingHotkeyService),
                    e
                );
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            HotkeyManager.Current.Remove(ToggleRecordingHotkeyName);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers or replaces the toggle-recording hotkey based on configuration.
        /// </summary>
        private void SetupHotkey() =>
            HotkeyManager.Current.AddOrReplace(
                ToggleRecordingHotkeyName,
                GetKey(),
                GetModifierKeys(),
                (
                    _,
                    _) =>
                {
                    if (transcriber.RecordingState != RecordingState.Uninitialized)
                        transcriber.ToggleRecording();
                }
            );

        /// <summary>
        /// Parses the configured key string into a <see cref="Key"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the key string is invalid.</exception>
        private Key GetKey()
        {
            if (!Enum.TryParse(
                    hotkeysSettings.Value.ToggleRecording.Key,
                    true,
                    out Key key
                ))
                throw new InvalidOperationException($"Invalid Hotkeys: Key '{hotkeysSettings.Value.ToggleRecording.Key}'");

            return key;
        }

        /// <summary>
        /// Parses configured modifier key strings into a combined <see cref="ModifierKeys"/> value.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if any modifier string is invalid.</exception>
        private ModifierKeys GetModifierKeys()
        {
            ModifierKeys result = ModifierKeys.None;
            foreach (string modifier in hotkeysSettings.Value.ToggleRecording.Modifiers)
            {
                if (!Enum.TryParse(
                        modifier,
                        true,
                        out ModifierKeys parsed
                    ))
                    throw new InvalidOperationException($"Invalid Hotkeys: Modifier '{modifier}'");

                result |= parsed;
            }

            return result;
        }
    }
}