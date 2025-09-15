using Microsoft.Extensions.Options;
using NHotkey;
using NHotkey.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ToolBuddy.LocalSpeechTranscriber.Services;
using ToolBuddy.LocalSpeechTranscriber.Settings;
using ToolBuddy.LocalSpeechTranscriber.ViewModels;

namespace ToolBuddy.LocalSpeechTranscriber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _toggleRecordingHotkeyName = "ToggleRecording";
        private readonly Transcriber _transcriber;
        private readonly HotkeySetting _toggleRecordingHotkeysSettings;

        public MainWindow(
            MainViewModel viewModel,
            Transcriber transcriber,
            IOptions<HotkeysSettings> hotkeysSettings)
        {
            _transcriber = transcriber;
            _toggleRecordingHotkeysSettings = hotkeysSettings.Value.ToggleRecording;
            DataContext = viewModel;

            InitializeComponent();
            Closing += MainWindow_Closing;

            //todo shouldn't SetupHotkey be in another class?
            try
            {
                SetupHotkey();
            }
            catch (HotkeyAlreadyRegisteredException e)
            {
                //todo handle error
                Console.WriteLine("Hotkey already registered.");
            }
        }


        private void SetupHotkey()
        {
            ModifierKeys modifierKeys = _toggleRecordingHotkeysSettings.Modifiers.Aggregate(
                ModifierKeys.None,
                (
                    current,
                    modifier) => current | modifier
            );

            HotkeyManager.Current.AddOrReplace(
                _toggleRecordingHotkeyName,
                _toggleRecordingHotkeysSettings.Key,
                modifierKeys,
                (_, _) => {
                    if (_transcriber.IsInitialized)
                        _transcriber.ToggleRecording();
                }
            );
        }

        private void MainWindow_Closing(
            object? sender,
            CancelEventArgs e)
        {
            if (DataContext is IDisposable disposable)
                disposable.Dispose();
            HotkeyManager.Current.Remove(_toggleRecordingHotkeyName);
        }
    }
}