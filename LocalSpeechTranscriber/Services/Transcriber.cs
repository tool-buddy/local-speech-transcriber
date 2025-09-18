using NAudio.Wave;
using ToolBuddy.LocalSpeechTranscriber.Services.Audio;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Services.Input;
using ToolBuddy.LocalSpeechTranscriber.Services.Stt;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class Transcriber(
        IAudioRecorder audioRecorder,
        ISttEngine sttEngine,
        IKeyboardOutput keyboardOutput,
        IErrorDisplayer errorDisplayer)
        : IDisposable
    {

        public RecordingState RecordingState { get; private set; } = RecordingState.Initializing;

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
            RecordingState = RecordingState.Idle;
            Initialized?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        private async void OnAudioDataAvailable(
            object? sender,
            WaveInEventArgs args)
        {
            try
            {
                await sttEngine.TranscribeAsync(
                    args.Buffer,
                    args.BytesRecorded
                ).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                errorDisplayer.Exception(
                    nameof(Transcriber),
                    e
                );
            }
        }

        private void OnSpeechTranscribed(
            object? sender,
            string transcribedText) =>
            keyboardOutput.TypeText(transcribedText);


        public void ToggleRecording()
        {
            if (RecordingState == RecordingState.Initializing)
                throw new InvalidOperationException("Transcriber is not initialized.");

            RecordingState = RecordingState == RecordingState.Recording
                ? RecordingState.Idle
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