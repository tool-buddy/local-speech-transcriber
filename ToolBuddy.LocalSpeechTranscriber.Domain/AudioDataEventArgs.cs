namespace ToolBuddy.LocalSpeechTranscriber.Domain
{
    /// <summary>
    /// Event data that contains a chunk of raw audio bytes.
    /// </summary>
    /// <param name="buffer">The buffer containing the audio bytes.</param>
    /// <param name="bytesRecorded">The number of valid bytes in the <paramref name="buffer"/>.</param>
    public sealed class AudioDataEventArgs(
        byte[] buffer,
        int bytesRecorded) : EventArgs
    {
        /// <summary>
        /// Gets the recorded audio data buffer.
        /// </summary>
        public byte[] Buffer { get; } = buffer;

        /// <summary>
        /// Gets the number of bytes recorded in the <see cref="Buffer"/>.
        /// </summary>
        public int BytesRecorded { get; } = bytesRecorded;
    }
}