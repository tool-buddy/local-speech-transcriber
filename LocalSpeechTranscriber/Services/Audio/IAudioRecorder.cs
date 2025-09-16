using NAudio.Wave;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Audio
{
    public interface IAudioRecorder
    {
        event EventHandler<WaveInEventArgs>? DataAvailable;
        void Start();
        void Stop();
    }
}