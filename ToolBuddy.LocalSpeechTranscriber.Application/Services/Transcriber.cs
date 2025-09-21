using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Services
{
    /// <summary>
    /// Coordinates audio recording and speech-to-text processing, and sends the transcribed text to the active window.
    /// </summary>
    /// <param name="audioRecorder">Component that captures audio data and raises data-available events.</param>
    /// <param name="sttEngine">Speech-to-text engine that produces transcription from audio buffers.</param>
    /// <param name="keyboardOutput">Service that types the recognized text into the active input control.</param>
    /// <param name="userNotifier">Service to notify users about errors and important events.</param>
    public sealed class Transcriber(
        IAudioRecorder audioRecorder,
        ITranscriptionEngine sttEngine,
        IKeyboardOutput keyboardOutput,
        IUserNotifier userNotifier)
        : IDisposable
    {
        private readonly SemaphoreSlim _transcribeLock = new(
            1,
            1
        );

        /// <summary>
        /// Gets the current state of the recording lifecycle.
        /// </summary>
        public RecordingState RecordingState { get; private set; } = RecordingState.Uninitialized;

        /// <inheritdoc />
        public void Dispose()
        {
            sttEngine.Initialized -= OnSttEngineInitialized;
            audioRecorder.DataAvailable -= OnAudioDataAvailable;
            sttEngine.Transcribed -= OnSpeechTranscribed;
        }

        /// <summary>
        /// Occurs when the transcriber has completed initialization and is ready.
        /// </summary>
        public event EventHandler? Initialized;

        /// <summary>
        /// Occurs when recording has started.
        /// </summary>
        public event EventHandler? RecordingStarted;

        /// <summary>
        /// Occurs when recording has stopped.
        /// </summary>
        public event EventHandler? RecordingStopped;

        /// <summary>
        /// Initializes the underlying speech-to-text engine.
        /// </summary>
        public void Initialize()
        {
            sttEngine.Initialized += OnSttEngineInitialized;
            sttEngine.Initialize();
        }

        /// <summary>
        /// Handles the speech-to-text engine initialization event.
        /// Subscribes to recorder and engine events and transitions to Idle state.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSttEngineInitialized(
            object? sender,
            EventArgs e)
        {
            audioRecorder.DataAvailable += OnAudioDataAvailable;
            sttEngine.Transcribed += OnSpeechTranscribed;
            RecordingState = RecordingState.Ready;
            Initialized?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        /// <summary>
        /// Handles incoming audio data by invoking the STT engine while preventing concurrent transcriptions.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The audio data payload.</param>
        /// <remarks>
        /// If a previous transcription is still in progress, the new audio chunk is dropped and an error notification is raised.
        /// </remarks>
        private async void OnAudioDataAvailable(
            object? sender,
            AudioDataEventArgs args)
        {
            bool acquired = false;
            try
            {
                // todo define the waiting time once you switch to the new transcription engine. Also consider the TODO in NAudioRecorder.OnWaveDataAvailable
                acquired = await _transcribeLock.WaitAsync(0).ConfigureAwait(false);
                if (acquired)
                    await sttEngine.TranscribeAsync(
                        args.Buffer,
                        args.BytesRecorded
                    ).ConfigureAwait(false);
                else
                    userNotifier.NotifyError(
                        nameof(Transcriber),
                        "Multiple transcriptions attempted, dropping audio data."
                    );
            }
            catch (Exception e)
            {
                userNotifier.NotifyError(
                    nameof(Transcriber),
                    e
                );
            }
            finally
            {
                if (acquired)
                    _transcribeLock.Release();
            }
        }

        /// <summary>
        /// Handles transcription results from the STT engine by typing the text into the active window.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="transcribedText">The recognized text.</param>
        private void OnSpeechTranscribed(
            object? sender,
            string transcribedText) =>
            keyboardOutput.TypeText(transcribedText);

        /// <summary>
        /// Toggles the recording state between <see cref="RecordingState.Recording"/> and <see cref="Domain.RecordingState.Ready"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if called before initialization completes.</exception>
        public void ToggleRecording()
        {
            if (RecordingState == RecordingState.Uninitialized)
                throw new InvalidOperationException("Transcriber is not initialized.");

            RecordingState = RecordingState == RecordingState.Recording
                ? RecordingState.Ready
                : RecordingState.Recording;

            if (RecordingState == RecordingState.Recording)
                StartRecording();
            else
                StopRecording();
        }

        /// <summary>
        /// Starts the audio recorder and raises <see cref="RecordingStarted"/>.
        /// </summary>
        private void StartRecording()
        {
            audioRecorder.Start();
            RecordingStarted?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        /// <summary>
        /// Stops the audio recorder and raises <see cref="RecordingStopped"/>.
        /// </summary>
        private void StopRecording()
        {
            audioRecorder.Stop();
            RecordingStopped?.Invoke(
                this,
                EventArgs.Empty
            );
        }
    }
}