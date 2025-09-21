using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    /// <summary>
    /// Base class for launching and supervising a Python-based Whisper server process.
    /// </summary>
    /// <param name="port">The port the server should listen on.</param>
    /// <param name="whisperModel">The Whisper model identifier to load.</param>
    /// <param name="pythonLocator">Service used to resolve a Python interpreter path.</param>
    /// <param name="logger">Logger used for diagnostic messages from the server process.</param>
    internal abstract class WhisperServerBase(
        int port,
        string whisperModel,
        IPythonLocator pythonLocator,
        ILogger logger)
        : IDisposable
    {
        private Process? _serverProcess;

        /// <summary>
        /// Gets the TCP port that the server will listen on.
        /// </summary>
        public int Port => port;

        /// <summary>
        /// Gets the Whisper model identifier used by the server.
        /// </summary>
        public string WhisperModel => whisperModel;

        /// <inheritdoc />
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

        /// <summary>
        /// Raised when the server indicates it is listening for connections.
        /// </summary>
        public event EventHandler? Started;

        /// <summary>
        /// Starts the Whisper server process and hooks output/error streams.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the server process fails to start.</exception>
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

        /// <summary>
        /// Creates and starts the external Python Whisper server process.
        /// </summary>
        /// <returns>The created <see cref="Process"/> instance, or null if the process could not be started.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no Python interpreter can be located.</exception>
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

        /// <summary>
        /// Gets the arguments to pass to the Python process to launch the Whisper server.
        /// </summary>
        /// <returns>The complete argument string.</returns>
        protected abstract string GetProcessArguments();

        /// <summary>
        /// Handles unexpected server process exit.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event args.</param>
        /// <exception cref="InvalidOperationException">Always thrown to indicate the server terminated.</exception>
        private void OnProcessExited(
            object? sender,
            EventArgs e) =>
            throw new InvalidOperationException("Whisper Streaming server process has exited.");

        /// <summary>
        /// Logs standard output lines from the server process.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The data received event arguments.</param>
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

        /// <summary>
        /// Logs standard error lines from the server process and detects readiness/critical errors.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The data received event arguments.</param>
        /// <exception cref="InvalidOperationException">Thrown when the process reports critical or error output.</exception>
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