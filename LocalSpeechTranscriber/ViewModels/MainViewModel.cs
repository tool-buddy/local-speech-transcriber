using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ToolBuddy.LocalSpeechTranscriber.Services;

namespace ToolBuddy.LocalSpeechTranscriber.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Transcriber _transcriber;
        private readonly BitmapImage _notRecordingIcon = new(new Uri("pack://application:,,,/Assets/not-recording.png"));
        private readonly BitmapImage _recordingIcon = new(new Uri("pack://application:,,,/Assets/recording.png"));

        private bool _isRecording;

        private bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording == value)
                    return;

                _isRecording = value;
                OnPropertyChanged(nameof(RecordButtonText));
                OnPropertyChanged(nameof(RecordButtonIcon));
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public string StatusText
        {
            get
            {
                string result;
                if (!_transcriber.IsInitialized)
                    result = "Initializing transcriber...";
                else if (!IsRecording)
                    result = "Ready";
                else if (IsRecording)
                    result = "Recording...";
                else
                    result = "Unknown status";
                return result;
            }
        }

        public string TranscriptionText =>
            _transcriber.TranscriptionText;

        public string RecordButtonText => IsRecording
            ? "Stop Recording"
            : "Start Recording";

        public BitmapImage RecordButtonIcon => IsRecording
            ? _recordingIcon
            : _notRecordingIcon;

        public bool CanRecord =>
            _transcriber.IsInitialized;

        public ICommand ToggleRecordingCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(
            Transcriber transcriber)
        {
            _transcriber = transcriber;
            _transcriber.Initialized += (
                _,
                _) =>
            {
                OnPropertyChanged(nameof(StatusText));
            };

            _transcriber.RecordingStarted += (
                _,
                _) =>
            {
                IsRecording = true;
            };
            _transcriber.RecordingStopped += (
                _,
                _) =>
            {
                IsRecording = false;
            };

            _transcriber.TextTyped += (
                _,
                _) =>
            {
                OnPropertyChanged(nameof(TranscriptionText));
            };

            ToggleRecordingCommand = new RelayCommand(
                _ => ToggleRecording(),
                _ => CanRecord
            );
        }


        private void ToggleRecording() =>
            _transcriber.ToggleRecording();


        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
    }
}