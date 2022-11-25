using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Teltonika.Codec;
using Teltonika.Codec.Model;
using System.IO;
using System.Reflection;

namespace TcpListener
{
    public class TcpClientService : IDisposable
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private byte[] _buffer = new byte[4096];
        public string RemoteEndPoint { get; }

        public TcpClientService(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            RemoteEndPoint = _client.Client.RemoteEndPoint.ToString();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            Log.Info("Received connection request from " + _client.Client.RemoteEndPoint);

            if (!await ReadImei(cancellationToken))
                return;

            await _stream.WriteAsync(new byte[] { 0x01 }, 0, 1, cancellationToken);

            await ReadData(cancellationToken);
        }

        private async Task ReadData(CancellationToken cancellationToken)
        {
            while (_client.Connected)
            {
                // read first byte
                var result = await Fill(0, 1, cancellationToken);
                if (!result) return;

                var firstByte = _buffer[0];
                if (firstByte == 0xFF) // ping
                {
                    Log.Info($"Received PING: {Utilities.ToHexString(_buffer, 0, 1)}"); // + preamble and crc
                    _buffer[0] = 0;
                    continue;
                }

                result = await Fill(1, 3 + 4, cancellationToken);
                if (!result) return;

                var preamble = BytesSwapper.Swap(BitConverter.ToInt32(_buffer, 0));
                var length = BytesSwapper.Swap(BitConverter.ToInt32(_buffer, 4)) + 4; // + 4 crc bytes

                if (preamble != 0)
                {
                    throw new NotSupportedException();
                }

                result = await Fill(8, length, cancellationToken);
                if (!result) return;

                if (!TryDecodeTcpPacket(_buffer, out var packet)) continue;

                if (packet.codecId == 12)
                {
                    Log.Info($"Received PING: {Utilities.ToHexString(_buffer, 0, length + 8)}"); // + preamble and crc
                    continue;
                }

                DecodeDataService decoderService = new DecodeDataService(Utilities.ToHexString(_buffer, 0, length + 8), true);
                try
                {
                    await decoderService.Run();
                }
                catch (Exception ex)
                {
                    Log.Error("Exception : " + ex.Message);
                }
                finally
                {
                    Log.Info("End of decoding");
                }

                Log.Info($"Received: {Utilities.ToHexString(_buffer, 0, length + 8)}"); // + preamble and crc
                await _stream.WriteAsync(new[] { (byte)packet.AvlData.Data.Count() }, 0, 1, cancellationToken);
            }
            Log.Error("Unexpected connection close.");
        }

        private async Task<bool> ReadImei(CancellationToken cancellationToken)
        {
            // read imei length
            await Fill(0, 2, cancellationToken);
            var imeiLength = _buffer[1];

            // read imei
            await Fill(0, imeiLength, cancellationToken);
            var imeiString = Encoding.ASCII.GetString(_buffer, 0, 15);
            if (!long.TryParse(imeiString, out var imei))
            {
                return false;
            }
            Log.Info($"Connected: [{imei}]");
            return true;
        }

        private async Task<bool> Fill(int offset, int count, CancellationToken cancellationToken)
        {
            if (offset + count > _buffer.Length)
            {
                var newSize = (offset + count) * 2;
                Array.Resize(ref _buffer, newSize);
            }

            var left = count;
            while (left > 0)
            {
                var read = await _stream.ReadAsync(_buffer, offset, left, cancellationToken);
                left = count - read;

                if (read == 0)
                {
                    Dispose();
                    throw new TaskCanceledException();
                }
            }

            return true;
        }

        private static bool TryDecodeTcpPacket(byte[] packetBytes, out TcpDataPacket packet)
        {
            packet = null;
            try
            {
                using var reader = new ReverseBinaryReader(new MemoryStream(packetBytes));
                var decoder = new DataDecoder(reader);

                packet = decoder.DecodeTcpData();

                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Error occured while decoding tcp packet", e);
                return false;
            }
        }

        public void Dispose()
        {
            ((IDisposable)_client)?.Dispose();
            _stream?.Dispose();
        }
    }
}