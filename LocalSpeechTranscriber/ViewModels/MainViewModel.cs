using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolBuddy.LocalSpeechTranscriber.Domain;
using ToolBuddy.LocalSpeechTranscriber.Services;
using ToolBuddy.LocalSpeechTranscriber.Services.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.ViewModels
{
    public sealed partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IMainThreadDispatcher _uiDispatcher;
        private readonly Transcriber _transcriber;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RecordButtonText))]
        private RecordingState _recordingState = RecordingState.Initializing;

        private bool IsInitialized => RecordingState != RecordingState.Initializing;

        [RelayCommand(CanExecute = nameof(IsInitialized))]
        private void ToggleRecording() => _transcriber.ToggleRecording();

        public string RecordButtonText =>
            RecordingState switch
            {
                RecordingState.Initializing => "Initializing...",
                RecordingState.Ready => "Start Recording",
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
            _transcriber.RecordingStarted += OnRecordingStarted;
            _transcriber.RecordingStopped += OnRecordingStopped;
        }

        public void Dispose()
        {
            _transcriber.Initialized -= OnTranscriberInitialized;
            _transcriber.RecordingStarted -= OnRecordingStarted;
            _transcriber.RecordingStopped -= OnRecordingStopped;
        }

        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() =>
                {
                    RecordingState = RecordingState.Ready;
                    ToggleRecordingCommand.NotifyCanExecuteChanged();
                }
            );

        private void OnRecordingStarted(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() => RecordingState = RecordingState.Recording);

        private void OnRecordingStopped(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() => RecordingState = RecordingState.Ready);
    }
}