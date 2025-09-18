using NAudio.Wave;
using ToolBuddy.LocalSpeechTranscriber.Services.Audio;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Services.Input;
using ToolBuddy.LocalSpeechTranscriber.Services.Stt;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class Transcriber(
        IAudioRecorder audioRecorder,
        ISttEngine sttEngine,
        IKeyboardOutput keyboardOutput,
        IErrorDisplayer errorDisplayer)
        : IDisposable
    {
        private bool _isRecording;

        public bool IsInitialized { get; private set; }

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
            IsInitialized = true;
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
            _isRecording = !_isRecording;

            if (_isRecording)
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