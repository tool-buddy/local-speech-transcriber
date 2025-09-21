namespace ToolBuddy.LocalSpeechTranscriber.Application.Options
{
    /// <summary>
    /// Selects the underlying Whisper-based speech-to-text implementation.
    /// </summary>
    public enum WhisperImplementationKind
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