using NAudio.Wave;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;
using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Audio
{
    /// <summary>
    /// NAudio-based implementation of <see cref="IAudioRecorder"/>.
    /// </summary>
    public sealed class NAudioRecorder : IAudioRecorder, IDisposable
    {
        private readonly WaveInEvent _waveIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="NAudioRecorder"/> class.
        /// </summary>
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

        /// <inheritdoc />
        public event EventHandler<AudioDataEventArgs>? DataAvailable;

        /// <inheritdoc />
        public void Start() => _waveIn.StartRecording();

        /// <inheritdoc />
        public void Stop() => _waveIn.StopRecording();

        /// <inheritdoc />
        public void Dispose()
        {
            _waveIn.DataAvailable -= OnWaveDataAvailable;
            _waveIn.Dispose();
        }

        /// <summary>
        /// Handles incoming audio buffers from NAudio and raises <see cref="DataAvailable"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The NAudio buffer payload.</param>
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