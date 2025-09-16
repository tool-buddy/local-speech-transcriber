using System.Diagnostics;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Stt
{
    public sealed partial class WhisperStreamingSttEngine
    {
        private sealed class Server(
            int port,
            string whisperModel,
            string pythonExecutable) : IDisposable
        {
            private Process? _serverProcess;
            public int Port => port;


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
                ProcessStartInfo startInfo = new()
                {
                    FileName = pythonExecutable,
                    Arguments =
                        $@".\whisper_streaming\whisper_online_server.py --model {whisperModel} --port {port} --vad",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                return Process.Start(startInfo);
            }

            private void OnProcessExited(
                object? sender,
                EventArgs e) =>
                throw new InvalidOperationException("Whisper Streaming server process has exited.");

            private void OnOutputDataReceived(
                object sender,
                DataReceivedEventArgs args) =>
                Debug.WriteLine($"Server stdout: {args.Data}");

            private void OnErrorDataReceived(
                object sender,
                DataReceivedEventArgs args)
            {
                Debug.WriteLine($"Server stderr: {args.Data}");

                if (args.Data != null)
                {
                    if (args.Data.Contains("critical") || args.Data.Contains("Error") || args.Data.Contains("error"))
                        throw new InvalidOperationException(args.Data);

                    if (args.Data.Contains($"Listening on('localhost', {port})"))
                        Started
                            ?.Invoke(
                                this,
                                EventArgs.Empty
                            );
                }
            }
        }
    }
}