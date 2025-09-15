using GregsStack.InputSimulatorStandard;
using NAudio.Wave;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class Transcriber : IDisposable
    {
        private readonly AudioRecorder _audioRecorder;
        private readonly InputSimulator _inputSimulator;
        private readonly IErrorDisplayer _errorDisplayer;
        private readonly ISttEngine _sttEngine;
        private bool _isRecording;

        //todo just for debugging
        public string TranscriptionText { get; private set; } = string.Empty;

        public bool IsInitialized { get; private set; }

        public event EventHandler? Initialized;
        public event EventHandler? RecordingStarted;
        public event EventHandler? RecordingStopped;
        public event EventHandler<string>? TextTyped;

        public Transcriber(
            AudioRecorder audioRecorder,
            ISttEngine sttEngine,
            InputSimulator inputSimulator,
            IErrorDisplayer errorDisplayer)

        {
            _audioRecorder = audioRecorder;
            _sttEngine = sttEngine;
            _inputSimulator = inputSimulator;
            _errorDisplayer = errorDisplayer;
        }


        public void Initialize()
        {
            _sttEngine.Initialized += OnSttEngineInitialized;
            _sttEngine.Initialize();
        }

        private void OnSttEngineInitialized(
            object? sender,
            EventArgs e)
        {
            _audioRecorder.DataAvailable += OnAudioDataAvailable;
            _sttEngine.Transcribed += OnSpeechTranscribed;
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
            //todo async void not good
            try
            {
                await _sttEngine.TranscribeAsync(
                    args.Buffer,
                    args.BytesRecorded
                ).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _errorDisplayer.Exception(
                    nameof(Transcriber),
                    e
                );
            }
        }

        private void OnSpeechTranscribed(
            object? sender,
            string transcribedText)
        {
            List<string> tokens = transcribedText.Split(
                [' '],
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();
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
                _inputSimulator.Keyboard.TextEntry(newText);
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
            _audioRecorder.Start();
            RecordingStarted?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        private void StopRecording()
        {
            _audioRecorder.Stop();
            RecordingStopped?.Invoke(
                this,
                EventArgs.Empty
            );
        }

        public void Dispose()
        {
            _sttEngine.Initialized -= OnSttEngineInitialized;
            _audioRecorder.DataAvailable -= OnAudioDataAvailable;
            _sttEngine.Transcribed -= OnSpeechTranscribed;
        }
    }
}