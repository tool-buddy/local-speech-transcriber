using Microsoft.Extensions.Logging;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    internal sealed class SimulStreamingServer(
        int port,
        string whisperModel,
        string pythonExecutable,
        ILogger logger)
        : BaseWhisperServer(
            port,
            whisperModel,
            pythonExecutable,
            logger
        )
    {
        protected override string GetProcessArguments() =>
            $@".\Stt\Whisper\Resources\SimulStreaming\simulstreaming_whisper_server.py --model_path {WhisperModel}.pt --port {Port} --lan auto --task transcribe --warmup-file .\Stt\Whisper\Resources\jfk.flac";
    }
}