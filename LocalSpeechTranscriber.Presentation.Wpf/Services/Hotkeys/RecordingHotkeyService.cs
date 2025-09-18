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
    public sealed class RecordingHotkeyService(
        IOptions<HotkeysSettings> hotkeysSettings,
        Transcriber transcriber,
        IUserNotifier userNotifier) : IHostedService
    {
        private const string ToggleRecordingHotkeyName = "ToggleRecording";

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                SetupHotkey();
            }
            catch (HotkeyAlreadyRegisteredException e)
            {
                userNotifier.Exception(
                    nameof(RecordingHotkeyService),
                    e
                );
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken)
        {
            HotkeyManager.Current.Remove(ToggleRecordingHotkeyName);
            return Task.CompletedTask;
        }

        private void SetupHotkey() =>
            HotkeyManager.Current.AddOrReplace(
                ToggleRecordingHotkeyName,
                GetKey(),
                GetModifierKeys(),
                (
                    _,
                    _) =>
                {
                    if (transcriber.RecordingState != RecordingState.Initializing)
                        transcriber.ToggleRecording();
                }
            );

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