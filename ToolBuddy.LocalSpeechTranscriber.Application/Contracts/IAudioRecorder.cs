using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    /// <summary>
    /// Abstraction for an audio recorder that captures audio input and exposes recorded data chunks.
    /// </summary>
    public interface IAudioRecorder
    {
        /// <summary>
        /// Occurs when a new chunk of audio data is available from the recorder.
        /// </summary>
        event EventHandler<AudioDataEventArgs>? DataAvailable;

        /// <summary>
        /// Starts capturing audio from the active input device.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops capturing audio.
        /// </summary>
        void Stop();
    }
}