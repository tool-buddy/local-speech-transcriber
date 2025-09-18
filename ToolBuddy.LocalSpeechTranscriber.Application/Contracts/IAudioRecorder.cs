using ToolBuddy.LocalSpeechTranscriber.Domain;

namespace ToolBuddy.LocalSpeechTranscriber.Application.Contracts
{
    public interface IAudioRecorder
    {
        event EventHandler<AudioDataEventArgs>? DataAvailable;

        void Start();
        void Stop();
    }
}