namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    /// <summary>
    /// Provides speech-to-text capabilities for audio buffers.
    /// </summary>
    public interface ITranscriber
    {
        /// <summary>
        /// Raised when a transcription is produced.
        /// </summary>
        /// <remarks>
        /// The string payload contains the text recognized from the audio.
        /// </remarks>
        event EventHandler<string>? Transcribed;

        /// <summary>
        /// Raised when the transcription engine has completed initialization and is ready to use.
        /// </summary>
        event EventHandler? Initialized;

        /// <summary>
        /// Initializes the engine.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Transcribes the provided audio buffer asynchronously.
        /// </summary>
        /// <param name="buffer">The audio bytes to transcribe.</param>
        /// <param name="bytesRecorded">The number of valid bytes in <paramref name="buffer"/>.</param>
        Task TranscribeAsync(
            byte[] buffer,
            int bytesRecorded);
    }
}