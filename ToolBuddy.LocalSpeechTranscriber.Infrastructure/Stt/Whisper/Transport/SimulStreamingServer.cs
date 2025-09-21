using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    /// <summary>
    /// Class for launching and supervising a Python-based SimulStreaming server process.
    /// </summary>
    internal sealed class SimulStreamingServer(
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
            $@".\Stt\Whisper\Resources\SimulStreaming\simulstreaming_whisper_server.py --model_path {WhisperModel}.pt --port {Port} --lan auto --task transcribe --warmup-file .\Stt\Whisper\Resources\jfk.flac";
    }
}