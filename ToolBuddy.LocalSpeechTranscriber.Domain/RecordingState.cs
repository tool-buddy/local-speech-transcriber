namespace ToolBuddy.LocalSpeechTranscriber.Domain
{
    /// <summary>
    /// Represents the current state of the audio recording lifecycle.
    /// </summary>
    public enum RecordingState
    {
        /// <summary>
        /// The recorder is not yet ready.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// The recorder is idle and ready to start.
        /// </summary>
        Ready,

        /// <summary>
        /// Audio is actively being recorded.
        /// </summary>
        Recording
    }
}