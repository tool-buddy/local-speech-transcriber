using NAudio.Wave;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Audio
{
    public sealed class AudioRecorder : IAudioRecorder, IDisposable
    {
        private readonly WaveInEvent _waveIn;

        public AudioRecorder()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(
                    16000,
                    16,
                    1
                ),
                BufferMilliseconds = 64 // 1024 samples, same as the python client
            };

            _waveIn.DataAvailable += OnWaveDataAvailable;
        }

        public void Dispose()
        {
            _waveIn.DataAvailable -= OnWaveDataAvailable;
            _waveIn.Dispose();
        }

        public event EventHandler<WaveInEventArgs>? DataAvailable;

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
    }
}