using System;
using Teltonika.DataParser.Client.Handlers.Codec;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Infrastructure.Factory;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Handlers
{
    public class PacketDecoder
    {
        public CompositeData DecodeTCPData(DataReader reader)
        {
            var parsedPacket = new CompositeData("TCP AVL Data Packet");
            parsedPacket.Add(reader.ReadData(4, DataType.Preamble));
            parsedPacket.Add(reader.ReadData(4, DataType.AvlDataArrayLength));
            parsedPacket.Add(DecodeAvlData(reader));

            parsedPacket.Add(reader.ReadData(4, DataType.Crc));

            return parsedPacket;
        }

        public CompositeData DecodeUdpData(DataReader reader)
        {
            var parsedPacket = new CompositeData("UDP AVL Data Packet");

            parsedPacket.Add(reader.ReadData(2, DataType.Length));
            parsedPacket.Add(reader.ReadData(2, DataType.PacketId));
            parsedPacket.Add(reader.ReadData(1, DataType.PacketType));
            parsedPacket.Add(reader.ReadData(1, DataType.AvlPacketId));

            var imeiLength = reader.ReadData(2, DataType.ImeiLength);
            parsedPacket.Add(imeiLength);
            parsedPacket.Add(reader.ReadData((byte)(int.Parse(imeiLength.Value)), DataType.Imei));
            parsedPacket.Add(DecodeAvlData(reader));

            return parsedPacket;
        }

        private CompositeData DecodeAvlData(DataReader reader)
        {
            var avlDataComposite = new CompositeData("Data");

            var codecId = reader.ReadData(1, DataType.CodecId);
            avlDataComposite.Add(codecId);

            var countData = reader.ReadData(1, DataType.AvlDataCount);
            avlDataComposite.Add(countData);

            AVLCodec avlDataHandler = AVLCodecFactory.CreateAVLCodec(codecId.Value);

            for (var i = 0; i < int.Parse(countData.Value); i++)
            {
                var avlData = avlDataHandler.GetAvlData(reader);
                avlDataComposite.Add(avlData);
            }

            avlDataComposite.Add(reader.ReadData(1, DataType.AvlDataCount));

            avlDataComposite.ConfigureArraySegment();

            return avlDataComposite;
        }
    }
}
