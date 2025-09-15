using System.Diagnostics;
using ToolBuddy.LocalSpeechTranscriber.Settings;
using ToolBuddy.LocalSpeechTranscriber.Extensions;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public partial class WhisperStreamingSttEngine
    {
        private sealed class Server : IDisposable
        {
            private readonly WhisperModel _model;
            private readonly IErrorDisplayer _errorDisplayer;
            private readonly int _port;
            private Process? _serverProcess;

            public event EventHandler? Started;
            public int Port => _port;


            public Server(
                int port,
                WhisperModel model,
                IErrorDisplayer errorDisplayer)
            {
                _port = port;
                _model = model;
                _errorDisplayer = errorDisplayer;
            }

            public void Dispose()
            {
                if (_serverProcess == null)
                    return;

                _serverProcess.ErrorDataReceived -= OnErrorDataReceived;
                _serverProcess.OutputDataReceived -= OnOutputDataReceived;

                //todo is this necessary?
                if (!_serverProcess.HasExited)
                    _serverProcess.Kill(true);
                _serverProcess?.Dispose();
            }

            public void Start()
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName =
                        "C:\\Users\\Aka\\AppData\\Local\\Programs\\Python\\Python38\\python.exe", // TODO: Make this configurable
                    Arguments =
                        $@".\whisper_streaming_main\whisper_online_server.py --model {_model.GetEnumMemberValue()} --port {_port} --vad", // TODO: Make this configurable
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                _serverProcess = Process.Start(startInfo);

                if (_serverProcess is null)
                    //todo handle error
                    throw new Exception("Failed to start server process.");

                _serverProcess.ErrorDataReceived += OnErrorDataReceived;
                _serverProcess.BeginErrorReadLine();

                _serverProcess.OutputDataReceived += OnOutputDataReceived;
                _serverProcess.BeginOutputReadLine();
            }

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
                        _errorDisplayer.Error(
                            nameof(Server),
                            args.Data
                        );

                    if (args.Data.Contains($"Listening on('localhost', {_port})"))
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