using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Transport
{
    /// <summary>
    /// TCP client that connects to the Whisper server, streams audio bytes, and raises transcription results.
    /// </summary>
    internal sealed class WhisperClient(IUserNotifier notifier, ILogger logger) : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Regex _serverResponsePattern = new(@"^(\d+)\s(\d+)\s(.*)$");
        private readonly TcpClient _tcpClient = new();

        private Task? _listeningTask;
        private NetworkStream? _networkStream;

        /// <inheritdoc />
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

        /// <summary>
        /// Raised when the server sends a transcription payload.
        /// </summary>
        public event EventHandler<string>? TranscriptionReceived;

        /// <summary>
        /// Connects to the Whisper server and starts a background task to read responses.
        /// </summary>
        /// <param name="host">The server host (e.g., "localhost").</param>
        /// <param name="port">The TCP port the server is listening on.</param>
        /// <exception cref="InvalidOperationException">Thrown if already connected or listening.</exception>
        /// <exception cref="NotSupportedException">Thrown if the connection attempt fails.</exception>
        public async Task ConnectAsync(
            string host,
            int port)
        {
            if (_networkStream != null)
                throw new InvalidOperationException($"{nameof(ConnectAsync)} already connected to server.");
            if (_listeningTask != null)
                throw new InvalidOperationException($"{nameof(WhisperClient)} already listening to server.");

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
                t => notifier.NotifyError(
                    nameof(WhisperClient),
                    t.Exception ?? new Exception("Unknown error")
                ),
                TaskContinuationOptions.OnlyOnFaulted
            );
        }

        /// <summary>
        /// Sends a chunk of audio bytes to the server for transcription.
        /// </summary>
        /// <param name="buffer">The buffer containing audio data.</param>
        /// <param name="bytesRecorded">The number of valid bytes to send from <paramref name="buffer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if the network stream is not initialized.</exception>
        public async Task SendAudioDataAsync(
            byte[] buffer,
            int bytesRecorded)
        {
            ArgumentNullException.ThrowIfNull(_networkStream);

            if (_tcpClient.Connected)
                await _networkStream.WriteAsync(
                    buffer,
                    0,
                    bytesRecorded
                ).ConfigureAwait(false);
            else
                logger.LogError("Attempted to send audio data while not connected to the server.");
        }

        /// <summary>
        /// Continuously reads responses from the server and raises <see cref="TranscriptionReceived"/>.
        /// </summary>
        /// <param name="networkStream">The network stream to read from.</param>
        /// <param name="cancellationToken">Token to cancel the listening loop.</param>
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

                string response = Encoding.UTF8.GetString(
                    buffer,
                    0,
                    bytesRead
                );

                TranscriptionReceived?.Invoke(
                    this,
                    GetTranscript(response)
                );
            }
        }

        /// <summary>
        /// Parses the server response and extracts the transcript text.
        /// </summary>
        /// <param name="response">The raw server response line.</param>
        /// <returns>The extracted transcript text.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the response does not match the expected pattern.</exception>
        private string GetTranscript(
            string response)
        {
            Match match = _serverResponsePattern.Match(response);
            if (!match.Success)
                throw new InvalidOperationException(
                    $"Server response did not follow the expected pattern. Response was '{response}'. Pattern is '{_serverResponsePattern}'"
                );

            return match.Groups[3].Value;
        }
    }
}