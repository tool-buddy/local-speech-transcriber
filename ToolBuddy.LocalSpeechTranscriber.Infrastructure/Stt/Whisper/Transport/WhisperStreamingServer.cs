using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    /// <summary>
    /// Class for launching and supervising a Python-based Whisper Streaming server process.
    /// </summary>
    internal sealed class WhisperStreamingServer(
        int port,
        string whisperModel,
        IPythonLocator pythonLocator,
        ILogger logger)
        : WhisperServerBase(
            port,
            whisperModel,
            pythonLocator,
            logger
        )
    {
        /// <inheritdoc />
        protected override string GetProcessArguments() =>
            $@".\Stt\Whisper\Resources\whisper_streaming\whisper_online_server.py --model {WhisperModel} --port {Port} --lan auto --task transcribe --warmup-file .\Stt\Whisper\Resources\jfk.flac";
    }
}