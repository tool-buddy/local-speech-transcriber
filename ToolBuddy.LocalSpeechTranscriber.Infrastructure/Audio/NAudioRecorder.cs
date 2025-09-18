using NAudio.Wave;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Audio
{
    public sealed class NAudioRecorder : IAudioRecorder, IDisposable
    {
        private readonly WaveInEvent _waveIn;

        public NAudioRecorder()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(
                    16000,
                    16,
                    1
                ),
                BufferMilliseconds = 64, // 1024 samples, same as the python client
                NumberOfBuffers = 20 // Increased number of buffers to reduce the chance of buffer overruns
            };

            _waveIn.DataAvailable += OnWaveDataAvailable;
        }

        public event EventHandler<AudioDataEventArgs>? DataAvailable;

        public void Start() => _waveIn.StartRecording();

        public void Stop() => _waveIn.StopRecording();

        public void Dispose()
        {
            _waveIn.DataAvailable -= OnWaveDataAvailable;
            _waveIn.Dispose();
        }

        private void OnWaveDataAvailable(
            object? sender,
            WaveInEventArgs e) =>
            //TODO the safe way would be to copy the buffer instead of using e.Buffer. The copying will have a performance impact though, except if you do a proper buffer pooling system, which will take more time to implement. For now, and considering how the rest of the app is implemented, I chose to keep the current solution while greatly reducing the risks of issues by setting a high value to _waveIn.NumberOfBuffers. This needs to be improved later when the app will mature.
            DataAvailable?.Invoke(
                this,
                new AudioDataEventArgs(
                    e.Buffer,
                    e.BytesRecorded
                )
            );
    }
}