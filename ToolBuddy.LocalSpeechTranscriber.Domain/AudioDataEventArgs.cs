namespace ToolBuddy.LocalSpeechTranscriber.Domain
{
    public sealed class AudioDataEventArgs(
        byte[] buffer,
        int bytesRecorded) : EventArgs
    {
        public byte[] Buffer { get; } = buffer;
        public int BytesRecorded { get; } = bytesRecorded;
    }
}