using System.Windows.Input;
using Microsoft.Extensions.Options;
using NHotkey;
using NHotkey.Wpf;
using ToolBuddy.LocalSpeechTranscriber.Settings;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class RecordingHotkeyToggler(IOptions<HotkeysSettings> hotkeysSettings, Transcriber transcriber) : IDisposable
    {
        private const string ToggleRecordingHotkeyName = "ToggleRecording";

        public void Initialize()
        {
            try
            {
                SetupHotkey();
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                //todo handle error
                Console.WriteLine("Hotkey already registered.");
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

        public void Dispose() =>
            HotkeyManager.Current.Remove(ToggleRecordingHotkeyName);
    }
}