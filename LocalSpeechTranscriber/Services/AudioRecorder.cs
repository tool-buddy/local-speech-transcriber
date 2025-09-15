using NAudio.Wave;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public sealed class AudioRecorder : IDisposable
    {
        private readonly WaveInEvent _waveIn;

        public event EventHandler<WaveInEventArgs>? DataAvailable;

        public AudioRecorder()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(
                    16000,
                    16,
                    1
                )
            };

            _waveIn.DataAvailable += OnWaveDataAvailable;
        }

        private void OnWaveDataAvailable(
            object? sender,
            WaveInEventArgs args) =>
            DataAvailable?.Invoke(
                sender,
                args
            );

        public void Start() =>
            _waveIn.StartRecording();

        public void Stop() =>
            _waveIn.StopRecording();

        public void Dispose()
        {
            _waveIn.DataAvailable -= OnWaveDataAvailable;
            _waveIn.Dispose();
        }
    }
}