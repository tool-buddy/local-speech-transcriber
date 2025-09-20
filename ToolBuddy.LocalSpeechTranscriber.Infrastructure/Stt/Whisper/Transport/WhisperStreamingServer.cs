using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    internal sealed class WhisperStreamingServer(
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
            $@".\Stt\Whisper\Resources\whisper_streaming\whisper_online_server.py --model {WhisperModel} --port {Port} --lan auto --task transcribe --warmup-file .\Stt\Whisper\Resources\jfk.flac";
    }
}