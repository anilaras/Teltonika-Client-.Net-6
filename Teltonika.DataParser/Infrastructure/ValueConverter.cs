using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.Codec;
using Teltonika.Codec.Model;
using Teltonika.Codec.Model.GH;

namespace Teltonika.DataParser.Client.Infrastructure
{
    public static class ValueConverter
    {
        private static readonly DateTime AvlEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime GHepoch = new DateTime(2007, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);


        public static string GetStringValue(ArraySegment<byte> arraySegment, DataType dataType)
        {
            var subArray = arraySegment.Array.Skip(arraySegment.Offset).Take(arraySegment.Count).ToArray();

            switch (dataType)
            {
                // Avl Data types
                case DataType.CodecId:
                case DataType.AvlDataCount:
                case DataType.Priority:
                case DataType.Satellites:
                case DataType.EventIoId:
                case DataType.IoCount:
                case DataType.IoCount1B:
                case DataType.IoId1B:
                case DataType.IoValue1B:
                case DataType.IoCount2B:
                case DataType.IoId2B:
                case DataType.IoCount4B:
                case DataType.IoId4B:
                case DataType.IoCount8B:
                case DataType.IoId8B:
                    return subArray[0].ToString();
                case DataType.Latitude:
                case DataType.Longitude:
                    return BytesSwapper.Swap(BitConverter.ToInt32(subArray, 0)).ToString();
                case DataType.IoValue4B:
                    return GetSignedUnsigned32(subArray);
                case DataType.Altitude:
                case DataType.Angle:
                case DataType.Speed:
                    return BytesSwapper.Swap(BitConverter.ToInt16(subArray, 0)).ToString();
                case DataType.IoValue2B:
                    return GetSignedUnsigned16(subArray);
                case DataType.IoValue8B:
                    return GetSignedUnsigned64(subArray);
                case DataType.Timestamp:
                    return AvlEpoch.AddMilliseconds(BytesSwapper.Swap(BitConverter.ToInt64(subArray, 0))).ToString();

                // Codec 8 Extended types
                case DataType.ExtendedIoCount1B:
                case DataType.ExtendedIoCount2B:
                case DataType.ExtendedIoCount4B:
                case DataType.ExtendedIoCount8B:
                case DataType.ExtendedIoCountXB:
                case DataType.ExtendedElementLength:
                case DataType.ExtendedEventIoId:
                case DataType.ExtendedIoCount:
                    return BytesSwapper.Swap(BitConverter.ToInt16(subArray, 0)).ToString();


                case DataType.ExtendedIoId1B:
                case DataType.ExtendedIoId2B:
                case DataType.ExtendedIoId4B:
                case DataType.ExtendedIoId8B:
                case DataType.ExtendedIoIdXB:
                    return GetSignedUnsigned16(subArray);
                case DataType.ExtendedIoValueXB:
                    return string.Empty;
                // Codec7 types
                case DataType.TimestampGh:
                    {
                        var priorityAndTimeStamp = BytesSwapper.Swap(BitConverter.ToInt32(subArray, 0));
                        var timeStamp = (long)(priorityAndTimeStamp & 0x3FFFFFFF);
                        return GHepoch.AddSeconds(timeStamp).ToString();
                    }
                case DataType.PriorityGh:
                    {
                        string priorityAndTimestampBits = BitConverters.ByteArrayToBits(subArray);
                        return ((GhAvlDataPriority)Convert.ToInt16(priorityAndTimestampBits.Substring(0, 2))).ToString();
                    }
                case DataType.GlobalMask:
                    return ((GlobalMaskCodec7)subArray[0]).ToString();
                case DataType.GpsElementMask:
                    return ((GpsElementMaskCodec7)subArray[0]).ToString();
                case DataType.LongitudeGh:
                case DataType.LatitudeGh:
                    {
                        var coordinate = BitConverters.EndianBitConverters.ToSingle(subArray, 0);
                        if (!GpsElement.IsLatValid(coordinate))
                        {
                            return 0.ToString();
                        }
                        return coordinate.ToString();
                    }
                case DataType.AngleGh:
                case DataType.SpeedGh:
                    return subArray[0].ToString();
                case DataType.CellIdAndLocalAreaGh:
                case DataType.OperatorCodeGh:
                    return BytesSwapper.Swap(BitConverter.ToInt32(subArray, 0)).ToString();
                case DataType.SignalQualityGh:
                    return subArray[0].ToString();
                // Codec16 types
                case DataType.EventIoIdCodec16:
                case DataType.IoId1BCodec16:
                case DataType.IoId2BCodec16:
                case DataType.IoId4BCodec16:
                case DataType.IoId8BCodec16:
                    return BytesSwapper.Swap(BitConverter.ToInt16(subArray, 0)).ToString();
                case DataType.OriginType:
                    return subArray[0].ToString();
                // Tcp types
                case DataType.Preamble:
                case DataType.AvlDataArrayLength:
                case DataType.Crc:
                    return GetSignedUnsigned32(subArray);
                // Udp types
                case DataType.Length:
                case DataType.PacketId:
                case DataType.ImeiLength:
                    return GetSignedUnsigned16(subArray);
                case DataType.PacketType:
                case DataType.AvlPacketId:
                    return subArray[0].ToString();
                case DataType.Imei:
                    return Encoding.UTF8.GetString(subArray);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetSignedUnsigned16(byte[] data)
        {
            var signed16 = BytesSwapper.Swap(BitConverter.ToInt16(data, 0)).ToString();
            var unsigned16 = BytesSwapper.Swap(BitConverter.ToUInt16(data, 0)).ToString();
            return signed16 == unsigned16 ? signed16 : string.Format("{0} / {1}", signed16, unsigned16);
        }

        private static string GetSignedUnsigned32(byte[] data)
        {
            var signed32 = BytesSwapper.Swap(BitConverter.ToInt32(data, 0)).ToString();
            var unsigned = BytesSwapper.Swap(BitConverter.ToUInt32(data, 0)).ToString();
            return signed32 == unsigned ? signed32 : string.Format("{0} / {1}", signed32, unsigned);
        }

        private static string GetSignedUnsigned64(byte[] data)
        {
            var signed64 = BytesSwapper.Swap(BitConverter.ToInt64(data, 0)).ToString();
            var unsigned64 = BytesSwapper.Swap(BitConverter.ToUInt64(data, 0)).ToString();
            return signed64 == unsigned64 ? signed64 : string.Format("{0} / {1}", signed64, unsigned64);
        }
    }
}
