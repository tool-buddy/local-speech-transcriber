using GregsStack.InputSimulatorStandard;
using NAudio.Wave;
using ToolBuddy.LocalSpeechTranscriber.Services.Audio;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;
using ToolBuddy.LocalSpeechTranscriber.Services.Stt;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class Transcriber(
        AudioRecorder audioRecorder,
        ISttEngine sttEngine,
        InputSimulator inputSimulator,
        IErrorDisplayer errorDisplayer)
        : IDisposable
    {
        private bool _isRecording;

        //todo just for debugging
        public string TranscriptionText { get; private set; } = string.Empty;

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
        public event EventHandler<string>? TextTyped;


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
            string transcribedText)
        {
            List<string> tokens =
            [
                .. transcribedText.Split(
                    [' '],
                    StringSplitOptions.RemoveEmptyEntries
                )
            ];

            while (tokens.Count > 0
                   && int.TryParse(
                       tokens[0],
                       out _
                   ))
                tokens.RemoveAt(0);

            string textOnly = string.Join(
                " ",
                tokens
            );

            if (!string.IsNullOrWhiteSpace(textOnly))
            {
                string newText = textOnly; //todo is space needed?
                TranscriptionText += newText;
                inputSimulator.Keyboard.TextEntry(newText);
                TextTyped?.Invoke(
                    this,
                    newText
                );
            }
        }


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