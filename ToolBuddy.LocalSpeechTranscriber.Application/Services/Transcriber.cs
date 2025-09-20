using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Services
{
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

        public RecordingState RecordingState { get; private set; } = RecordingState.Uninitialized;

        public void Dispose()
        {
            sttEngine.Initialized -= OnSttEngineInitialized;
            audioRecorder.DataAvailable -= OnAudioDataAvailable;
            sttEngine.Transcribed -= OnSpeechTranscribed;
        }

        public event EventHandler? Initialized;
        public event EventHandler? RecordingStarted;
        public event EventHandler? RecordingStopped;

        public void Initialize()
        {
            sttEngine.Initialized += OnSttEngineInitialized;
            sttEngine.Initialize();
        }

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

        private async void OnAudioDataAvailable(
            object? sender,
            AudioDataEventArgs args)
        {
            bool acquired = false;
            try
            {
                //todo define the waiting time once you switch to the new transcription engine. Also consider the TODO in NAudioRecorder.OnWaveDataAvailable
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

        private void OnSpeechTranscribed(
            object? sender,
            string transcribedText) =>
            keyboardOutput.TypeText(transcribedText);

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

        private void StartRecording()
        {
            audioRecorder.Start();
            RecordingStarted?.Invoke(
                this,
                EventArgs.Empty
            );
        }

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