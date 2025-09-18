using System.Windows.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NHotkey;
using NHotkey.Wpf;
using ToolBuddy.LocalSpeechTranscriber.Domain;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Settings;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Hotkeys
{
    public sealed class RecordingHotkeyToggler(
        IOptions<HotkeysSettings> hotkeysSettings,
        Transcriber transcriber,
        IErrorDisplayer errorDisplayer) : IHostedService, IDisposable
    {
        private const string ToggleRecordingHotkeyName = "ToggleRecording";

        public void Dispose() =>
            HotkeyManager.Current.Remove(ToggleRecordingHotkeyName);

        public Task StartAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                SetupHotkey();
            }
            catch (HotkeyAlreadyRegisteredException e)
            {
                errorDisplayer.Exception(
                    nameof(RecordingHotkeyToggler),
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
                hotkeysSettings.Value.ToggleRecording.Key,
                GetModifierKeys(),
                (
                    _,
                    _) =>
                {
                    if (transcriber.RecordingState != RecordingState.Initializing)
                        transcriber.ToggleRecording();
                }
            );

        private ModifierKeys GetModifierKeys() =>
            hotkeysSettings.Value.ToggleRecording.Modifiers.Aggregate(
                ModifierKeys.None,
                (
                    current,
                    modifier) => current | modifier
            );
    }
}