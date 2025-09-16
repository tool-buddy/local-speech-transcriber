using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using ToolBuddy.LocalSpeechTranscriber.Services.ErrorManagement;

namespace ToolBuddy.LocalSpeechTranscriber.Services.Stt
{
    public sealed partial class WhisperStreamingSttEngine
    {
        private sealed class Client(IErrorDisplayer errorDisplayer) : IDisposable
        {
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly TcpClient _tcpClient = new();
            private Task? _listeningTask;
            private NetworkStream? _networkStream;


            public void Dispose()
            {
                try
                {
                    try
                    {
                        _cancellationTokenSource.Cancel();
                    }
                    catch (ObjectDisposedException) { }

                    try
                    {
                        _listeningTask?.Wait();
                    }
                    catch (AggregateException e) when (e.InnerExceptions.All(ie => ie is OperationCanceledException)) { }
                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                    _networkStream?.Dispose();
                    _tcpClient.Dispose();
                }
            }

            public event EventHandler<string>? TranscriptionReceived;

            public async Task ConnectAsync(
                string host,
                int port)
            {
                if (_networkStream != null)
                    throw new InvalidOperationException($"{nameof(ConnectAsync)} already connected to server.");
                if (_listeningTask != null)
                    throw new InvalidOperationException($"{nameof(Client)} already listening to server.");

                await _tcpClient.ConnectAsync(
                    host,
                    port
                ).ConfigureAwait(false);

                if (!_tcpClient.Connected)
                    throw new NotSupportedException("Failed to connect to the transcription server.");

                _networkStream = _tcpClient.GetStream();

                CancellationToken cancellationToken = _cancellationTokenSource.Token;

                _listeningTask = ListenAsync(
                    _networkStream,
                    cancellationToken
                );

                _ = _listeningTask.ContinueWith(
                    t => errorDisplayer.Exception(
                        nameof(Client),
                        t.Exception ?? new Exception("Unknown error")
                    ),
                    TaskContinuationOptions.OnlyOnFaulted
                );
            }

            private async Task ListenAsync(
                NetworkStream networkStream,
                CancellationToken cancellationToken)
            {
                byte[] buffer = new byte[1024];
                while (_tcpClient.Connected && !cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await networkStream.ReadAsync(
                        buffer,
                        0,
                        buffer.Length,
                        cancellationToken
                    ).ConfigureAwait(false);

                    if (bytesRead == 0) break;

                    string text = Encoding.UTF8.GetString(
                        buffer,
                        0,
                        bytesRead
                    );

                    _ = Task.Run(
                        () =>
                        {
                            try
                            {
                                TranscriptionReceived?.Invoke(
                                    this,
                                    text
                                );
                            }
                            catch (Exception handlerEx)
                            {
                                Debug.WriteLine(
                                    $"{nameof(Client)}: TranscriptionReceived handler error: {handlerEx}"
                                );
                            }
                        },
                        CancellationToken.None
                    );
                }
            }

            public async Task SendAudioDataAsync(
                byte[] buffer,
                int bytesRecorded)
            {
                ArgumentNullException.ThrowIfNull(_networkStream);

                await _networkStream.WriteAsync(
                    buffer,
                    0,
                    bytesRecorded
                ).ConfigureAwait(false);
            }
        }
    }
}