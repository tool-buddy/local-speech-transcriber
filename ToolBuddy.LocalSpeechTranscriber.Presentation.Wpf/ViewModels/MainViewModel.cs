using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolBuddy.LocalSpeechTranscriber.Application.Orchestration;
using ToolBuddy.LocalSpeechTranscriber.Domain;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Abstractions;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.ViewModels
{
    /// <summary>
    /// View model for the main view.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IAppInfoProvider _appInfoProvider;
        private readonly TranscriptionOrchestrator _transcriptionOrchestrator;
        private readonly IUIDispatcher _uiDispatcher;

        /// <summary>
        /// Backing field for the current recording state.
        /// The generated property notifies changes for <see cref="RecordButtonText"/>.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RecordButtonText))]
        private RecordingState _recordingState = RecordingState.Uninitialized;

        /// <summary>
        /// Product name to display in the UI (e.g., window title).
        /// </summary>
        public string ProductName => _appInfoProvider.ProductName;

        private bool IsTranscriberInitialized => _transcriptionOrchestrator.RecordingState != RecordingState.Uninitialized;

        /// <summary>
        /// Gets the text displayed on the record/stop button based on <see cref="RecordingState"/>.
        /// </summary>
        public string RecordButtonText =>
            RecordingState switch
            {
                RecordingState.Uninitialized => "Initializing...",
                RecordingState.Ready => "Start Recording",
                RecordingState.Recording => "Stop Recording",
                _ => "Unknown state"
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="appInfoProvider">Provides product information for display.</param>
        /// <param name="transcriptionOrchestrator">Transcription orchestrator used to control recording.</param>
        /// <param name="uiDispatcher">Dispatcher used to marshal updates onto the UI thread.</param>
        public MainViewModel(
            IAppInfoProvider appInfoProvider,
            TranscriptionOrchestrator transcriptionOrchestrator,
            IUIDispatcher uiDispatcher)
        {
            _appInfoProvider = appInfoProvider;
            _uiDispatcher = uiDispatcher;
            _transcriptionOrchestrator = transcriptionOrchestrator;

            _transcriptionOrchestrator.Initialized += OnTranscriptionOrchestratorInitialized;
            _transcriptionOrchestrator.RecordingStarted += OnRecordingChanged;
            _transcriptionOrchestrator.RecordingStopped += OnRecordingChanged;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _transcriptionOrchestrator.Initialized -= OnTranscriptionOrchestratorInitialized;
            _transcriptionOrchestrator.RecordingStarted -= OnRecordingChanged;
            _transcriptionOrchestrator.RecordingStopped -= OnRecordingChanged;
        }

        /// <summary>
        /// Command that toggles the recording state. Only enabled once the transcriptionOrchestrator is initialized.
        /// </summary>
        [RelayCommand(CanExecute = nameof(IsTranscriberInitialized))]
        private void ToggleRecording() => _transcriptionOrchestrator.ToggleRecording();

        /// <summary>
        /// Handles the transcriptionOrchestrator initialization event by updating the UI state and command availability.
        /// </summary>
        private void OnTranscriptionOrchestratorInitialized(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() =>
                {
                    RecordingState = _transcriptionOrchestrator.RecordingState;
                    ToggleRecordingCommand.NotifyCanExecuteChanged();
                }
            );

        /// <summary>
        /// Handles start/stop events by updating the current <see cref="RecordingState"/>.
        /// </summary>
        private void OnRecordingChanged(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() => RecordingState = _transcriptionOrchestrator.RecordingState);
    }
}