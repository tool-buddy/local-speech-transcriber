using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ToolBuddy.LocalSpeechTranscriber.Services
{
    public partial class WhisperStreamingSttEngine
    {
        private sealed class Client : IDisposable
        {
            private NetworkStream? _networkStream;
            private readonly TcpClient _tcpClient = new();
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private Task? _listeningTask;
            public event EventHandler<string>? TranscriptionReceived;

            public async Task ConnectAsync(
                string host,
                int port)
            {
                await _tcpClient.ConnectAsync(
                    host,
                    port
                ).ConfigureAwait(false);

                if (!_tcpClient.Connected)
                    //todo handle error
                    throw new NotSupportedException("Failed to connect to the transcription server.");

                _networkStream = _tcpClient.GetStream();
            }

            public void StartListening()
            {
                //todo handle errors
                NetworkStream networkStream = _networkStream ?? throw new InvalidOperationException();
                ArgumentNullException.ThrowIfNull(networkStream); //todo exception is ignored in caller
                if (_listeningTask != null) //todo document exceptions
                    throw new InvalidOperationException("Already listening.");

                CancellationToken cancellationToken = _cancellationTokenSource.Token;

                _listeningTask = ListenAsync(
                    networkStream,
                    cancellationToken
                );

                // Optional safety net to make sure faults are observed even if the caller never awaits Completion.
                _ = _listeningTask.ContinueWith(
                    //todo handle error
                    t => Debug.WriteLine($"{nameof(Client)}: listener faulted: {t.Exception}"),
                    TaskContinuationOptions.OnlyOnFaulted
                );
            }

            private async Task ListenAsync(
                NetworkStream networkStream,
                CancellationToken cancellationToken)
            {
                byte[] buffer = new byte[1024];
                try
                {
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
                                    //todo handle error
                                    Debug.WriteLine($"{nameof(Client)}: TranscriptionReceived handler error: {handlerEx}");
                                }
                            },
                            CancellationToken.None
                        );
                    }
                }
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine($"{nameof(Client)}: Listening cancelled: {e.Message}");
                }
                catch (IOException e)
                {
                    //todo handle error
                    Debug.WriteLine($"{nameof(Client)}: Connection closed: {e.Message}");
                }
                catch (ObjectDisposedException e)
                {
                    Debug.WriteLine($"{nameof(Client)}: Object disposed while listening: {e.Message}");
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine($"{nameof(Client)}: Invalid operation while listening: {e.Message}");
                }
                catch (Exception e)
                {
                    //todo handle error
                    Debug.WriteLine($"{nameof(Client)}: Error receiving data: {e.Message}");
                }
            }

            public async Task SendAudioDataAsync(
                byte[] buffer,
                int bytesRecorded)
            {
                if (_networkStream is { CanWrite: true })
                    try
                    {
                        await _networkStream.WriteAsync(
                            buffer,
                            0,
                            bytesRecorded
                        ).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"{nameof(Client)}: Failed to send audio data: {e.Message}");
                        throw;
                    }
            }


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
        }
    }
}