using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolBuddy.LocalSpeechTranscriber.Application.Services;
using ToolBuddy.LocalSpeechTranscriber.Domain;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.AppInfo;
using ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.Services.Threading;

namespace ToolBuddy.LocalSpeechTranscriber.Presentation.Wpf.ViewModels
{
    /// <summary>
    /// View model for the main view.
    /// </summary>
    public sealed partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IAppInfo _appInfo;
        private readonly Transcriber _transcriber;
        private readonly IMainThreadDispatcher _uiDispatcher;

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
        public string ProductName => _appInfo.ProductName;

        private bool IsTranscriberInitialized => _transcriber.RecordingState != RecordingState.Uninitialized;

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
        /// <param name="appInfo">Provides product information for display.</param>
        /// <param name="transcriber">Transcriber service used to control recording.</param>
        /// <param name="uiDispatcher">Dispatcher used to marshal updates onto the UI thread.</param>
        public MainViewModel(
            IAppInfo appInfo,
            Transcriber transcriber,
            IMainThreadDispatcher uiDispatcher)
        {
            _appInfo = appInfo;
            _uiDispatcher = uiDispatcher;
            _transcriber = transcriber;

            _transcriber.Initialized += OnTranscriberInitialized;
            _transcriber.RecordingStarted += OnRecordingChanged;
            _transcriber.RecordingStopped += OnRecordingChanged;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _transcriber.Initialized -= OnTranscriberInitialized;
            _transcriber.RecordingStarted -= OnRecordingChanged;
            _transcriber.RecordingStopped -= OnRecordingChanged;
        }

        /// <summary>
        /// Command that toggles the recording state. Only enabled once the transcriber is initialized.
        /// </summary>
        [RelayCommand(CanExecute = nameof(IsTranscriberInitialized))]
        private void ToggleRecording() => _transcriber.ToggleRecording();

        /// <summary>
        /// Handles the transcriber initialization event by updating the UI state and command availability.
        /// </summary>
        private void OnTranscriberInitialized(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() =>
                {
                    RecordingState = _transcriber.RecordingState;
                    ToggleRecordingCommand.NotifyCanExecuteChanged();
                }
            );

        /// <summary>
        /// Handles start/stop events by updating the current <see cref="RecordingState"/>.
        /// </summary>
        private void OnRecordingChanged(
            object? sender,
            EventArgs e) =>
            _uiDispatcher.Post(() => RecordingState = _transcriber.RecordingState);
    }
}