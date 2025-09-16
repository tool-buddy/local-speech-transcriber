using System.Windows.Input;
using Microsoft.Extensions.Options;
using NHotkey;
using NHotkey.Wpf;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Settings;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Hotkeys
{
    public sealed class RecordingHotkeyToggler(
        IOptions<HotkeysSettings> hotkeysSettings,
        Transcriber transcriber,
        IErrorDisplayer errorDisplayer) : IDisposable
    {
        private const string ToggleRecordingHotkeyName = "ToggleRecording";

        public void Dispose() =>
            HotkeyManager.Current.Remove(ToggleRecordingHotkeyName);

        public void Initialize()
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
        }

        private void SetupHotkey()
        {
            ModifierKeys modifierKeys = hotkeysSettings.Value.ToggleRecording.Modifiers.Aggregate(
                ModifierKeys.None,
                (
                    current,
                    modifier) => current | modifier
            );

            HotkeyManager.Current.AddOrReplace(
                ToggleRecordingHotkeyName,
                hotkeysSettings.Value.ToggleRecording.Key,
                modifierKeys,
                (
                    _,
                    _) =>
                {
                    if (transcriber.IsInitialized)
                        transcriber.ToggleRecording();
                }
            );
        }
    }
}