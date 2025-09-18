using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolBuddy.LocalSpeechTranscriber.Domain;
using ToolBuddy.LocalSpeechTranscriber.Services;
using ToolBuddy.LocalSpeechTranscriber.Services.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.ViewModels
{
    public sealed partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly Transcriber _transcriber;
        private readonly IMainThreadDispatcher _uiDispatcher;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RecordButtonText))]
        private RecordingState _recordingState = RecordingState.Initializing;

        private bool IsTranscriberInitialized => _transcriber.RecordingState != RecordingState.Initializing;

        public string RecordButtonText =>
            RecordingState switch
            {
                RecordingState.Initializing => "Initializing...",
                RecordingState.Idle => "Start Recording",
                RecordingState.Recording => "Stop Recording",
                _ => "Unknown state"
            };

        public MainViewModel(
            Transcriber transcriber,
            IMainThreadDispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
            _transcriber = transcriber;

            _transcriber.Initialized += OnTranscriberInitialized;
            _transcriber.RecordingStarted += OnRecordingChanged;
            _transcriber.RecordingStopped += OnRecordingChanged;
        }

        public void Dispose()
        {
            _transcriber.Initialized -= OnTranscriberInitialized;
            _transcriber.RecordingStarted -= OnRecordingChanged;
            _transcriber.RecordingStopped -= OnRecordingChanged;
        }

        [RelayCommand(CanExecute = nameof(IsTranscriberInitialized))]
        private void ToggleRecording() => _transcriber.ToggleRecording();

        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() =>
                {
                    RecordingState = _transcriber.RecordingState;
                    ToggleRecordingCommand.NotifyCanExecuteChanged();
                }
            );

        private void OnRecordingChanged(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() => RecordingState = _transcriber.RecordingState);
    }
}