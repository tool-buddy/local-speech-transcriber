using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    internal sealed class SimulStreamingServer(
        int port,
        string whisperModel,
        IPythonLocator pythonLocator,
        ILogger logger)
        : BaseWhisperServer(
            port,
            whisperModel,
            pythonLocator,
            logger
        )
    {
        protected override string GetProcessArguments() =>
            $@".\Stt\Whisper\Resources\SimulStreaming\simulstreaming_whisper_server.py --model_path {WhisperModel}.pt --port {Port} --lan auto --task transcribe --warmup-file .\Stt\Whisper\Resources\jfk.flac";
    }
}