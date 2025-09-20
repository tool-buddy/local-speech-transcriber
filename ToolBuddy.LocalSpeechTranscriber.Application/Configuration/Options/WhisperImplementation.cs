namespace ToolBuddy.LocalSpeechTranscriber.Application.Configuration.Options
{
    /// <summary>
    /// Selects the underlying Whisper-based speech-to-text implementation.
    /// </summary>
    public enum WhisperImplementation
    {
        /// <summary>
        /// Uses the Whisper Streaming implementation.
        /// </summary>
        WhisperStreaming,

        /// <summary>
        /// Uses the SimulStreaming implementation.
        /// </summary>
        SimulStreaming
    }
}