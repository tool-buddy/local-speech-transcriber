using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ToolBuddy.LocalSpeechTranscriber.Services;

namespace ToolBuddy.LocalSpeechTranscriber.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Dispatcher _dispatcher;
        private readonly BitmapImage _notRecordingIcon;
        private readonly BitmapImage _recordingIcon;
        private readonly Transcriber _transcriber;
        private bool _isRecording;

        private bool IsRecording
        {
            get => _isRecording;
            set
            {
                if (_isRecording == value) return;

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

        public string RecordButtonText => IsRecording
            ? "Stop Recording"
            : "Start Recording";

        public BitmapImage RecordButtonIcon => IsRecording
            ? _recordingIcon
            : _notRecordingIcon;

        public string TranscriptionText => _transcriber.TranscriptionText;

        public bool CanRecord => _transcriber.IsInitialized;

        public RelayCommand ToggleRecordingCommand { get; }

        public MainViewModel(
            Transcriber transcriber)
        {
            _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            _transcriber = transcriber;
            _notRecordingIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/not-recording.png"));
            _recordingIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/recording.png"));
            _notRecordingIcon.Freeze();
            _recordingIcon.Freeze();

            _transcriber.Initialized += OnTranscriberInitialized;
            _transcriber.RecordingStarted += OnRecordingStarted;
            _transcriber.RecordingStopped += OnRecordingStopped;
            _transcriber.TextTyped += OnTextTyped;

            ToggleRecordingCommand = new RelayCommand(
                _ => _transcriber.ToggleRecording(),
                _ => CanRecord
            );
        }

        public void Dispose()
        {
            _transcriber.Initialized -= OnTranscriberInitialized;
            _transcriber.RecordingStarted -= OnRecordingStarted;
            _transcriber.RecordingStopped -= OnRecordingStopped;
            _transcriber.TextTyped -= OnTextTyped;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                new Action(() =>
                    {
                        OnPropertyChanged(nameof(CanRecord));
                        OnPropertyChanged(nameof(StatusText));
                        OnPropertyChanged(nameof(RecordButtonIcon));
                        OnPropertyChanged(nameof(RecordButtonText));

                        ToggleRecordingCommand.RaiseCanExecuteChanged();
                    }
                )
            );

        private void OnRecordingStarted(
            object? sender,
            EventArgs e) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                new Action(() => IsRecording = true)
            );

        private void OnRecordingStopped(
            object? sender,
            EventArgs e) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                new Action(() => IsRecording = false)
            );

        private void OnTextTyped(
            object? sender,
            string _) =>
            _dispatcher.BeginInvoke(
                DispatcherPriority.DataBind,
                new Action(() =>
                    OnPropertyChanged(nameof(TranscriptionText))
                )
            );

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
    }
}