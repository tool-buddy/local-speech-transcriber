using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    internal abstract class BaseWhisperServer(
        int port,
        string whisperModel,
        IPythonLocator pythonLocator,
        ILogger logger)
        : IDisposable
    {
        private Process? _serverProcess;

        public int Port => port;
        public string WhisperModel => whisperModel;


        public void Dispose()
        {
            if (_serverProcess == null)
                return;

            _serverProcess.Exited -= OnProcessExited;
            _serverProcess.ErrorDataReceived -= OnErrorDataReceived;
            _serverProcess.OutputDataReceived -= OnOutputDataReceived;

            try
            {
                if (!_serverProcess.HasExited)
                    _serverProcess.Kill(true);
            }
            finally
            {
                _serverProcess.Dispose();
                _serverProcess = null;
            }
        }

        public event EventHandler? Started;

        public void Start()
        {
            _serverProcess = CreateServerProcess();
            if (_serverProcess is null)
                throw new InvalidOperationException("Failed to start Whisper Streaming server process.");

            if (_serverProcess.HasExited)
                OnProcessExited(
                    _serverProcess,
                    EventArgs.Empty
                );
            else
                _serverProcess.Exited += OnProcessExited;

            _serverProcess.ErrorDataReceived += OnErrorDataReceived;
            _serverProcess.BeginErrorReadLine();

            _serverProcess.OutputDataReceived += OnOutputDataReceived;
            _serverProcess.BeginOutputReadLine();
        }

        private Process? CreateServerProcess()
        {
            if (!pythonLocator.TryGetPythonPath(
                    out string? pythonPath
                ))
                throw new InvalidOperationException(
                    "Could not locate a Python interpreter . Please install Python 3.9+ and ensure it is available on your PATH."
                );

            ProcessStartInfo startInfo = new()
            {
                FileName = pythonPath,
                Arguments = GetProcessArguments(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            logger.LogInformation(
                "Starting Whisper server: {FileName} {Args}",
                startInfo.FileName,
                startInfo.Arguments
            );
            return Process.Start(startInfo);
        }

        protected abstract string GetProcessArguments();

        private void OnProcessExited(
            object? sender,
            EventArgs e) =>
            throw new InvalidOperationException("Whisper Streaming server process has exited.");

        private void OnOutputDataReceived(
            object sender,
            DataReceivedEventArgs args)
        {
            if (args.Data is null) return;

            logger.LogInformation(
                "{Line}",
                args.Data
            );
        }

        private void OnErrorDataReceived(
            object sender,
            DataReceivedEventArgs args)
        {
            if (args.Data is null) return;

            logger.LogInformation(
                "{Line}",
                args.Data
            );

            if (args.Data.Contains(
                    "critical"
                )
                || args.Data.Contains(
                    "error"
                )
                || args.Data.Contains(
                    "Error"
                ))
                throw new InvalidOperationException(args.Data);

            if (args.Data.Contains($"Listening on('localhost', {port})"))
                Started?.Invoke(
                    this,
                    EventArgs.Empty
                );
        }
    }
}