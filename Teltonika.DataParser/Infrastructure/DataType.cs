using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teltonika.DataParser.Client.Infrastructure
{
    public enum DataType
    {
        Composite,
        [Display(Name = "Codec ID")]
        CodecId,
        [Display(Name = "AVL Data Count")]
        AvlDataCount,
        Timestamp,
        Priority,
        Latitude,
        Longitude,
        Altitude,
        Angle,
        Satellites,
        Speed,
        [Display(Name = "Event ID")]
        EventIoId,
        [Display(Name = "Element count")]
        IoCount,
        [Display(Name = "1b Element count")]
        IoCount1B,
        [Display(Name = "ID")]
        IoId1B,
        [Display(Name = "Value")]
        IoValue1B,
        [Display(Name = "2b Element count")]
        IoCount2B,
        [Display(Name = "ID")]
        IoId2B,
        [Display(Name = "Value")]
        IoValue2B,
        [Display(Name = "4b Element count")]
        IoCount4B,
        [Display(Name = "ID")]
        IoId4B,
        [Display(Name = "Value")]
        IoValue4B,
        [Display(Name = "8b Element Count")]
        IoCount8B,
        [Display(Name = "ID")]
        IoId8B,
        [Display(Name = "Value")]
        IoValue8B,
        // Tcp types
        Preamble,
        [Display(Name = "AVL Data Length")]
        AvlDataArrayLength,
        Crc,
        // Codec 8 Extended types
        [Display(Name = "1b Element count")]
        ExtendedIoCount1B,
        [Display(Name = "2b Element count")]
        ExtendedIoCount2B,
        [Display(Name = "4b Element count")]
        ExtendedIoCount4B,
        [Display(Name = "8b Element count")]
        ExtendedIoCount8B,
        [Display(Name = "Xb Element count")]
        ExtendedIoCountXB,
        [Display(Name = "Event ID")]
        ExtendedEventIoId,
        [Display(Name = "Element count")]
        ExtendedIoCount,
        [Display(Name = "ID")]
        ExtendedIoId1B,
        [Display(Name = "ID")]
        ExtendedIoId2B,
        [Display(Name = "ID")]
        ExtendedIoId4B,
        [Display(Name = "ID")]
        ExtendedIoId8B,
        [Display(Name = "ID")]
        ExtendedIoIdXB,
        [Display(Name = "Value")]
        ExtendedIoValueXB,
        [Display(Name = "Element length")]
        ExtendedElementLength,
        // Codec 16 types
        [Display(Name = "Event ID")]
        EventIoIdCodec16,
        [Display(Name = "Origin Type")]
        OriginType,
        [Display(Name = "ID")]
        IoId1BCodec16,
        [Display(Name = "ID")]
        IoId2BCodec16,
        [Display(Name = "ID")]
        IoId4BCodec16,
        [Display(Name = "ID")]
        IoId8BCodec16,
        // Codec7 types
        [Display(Name = "Priority")]
        PriorityGh,
        [Display(Name = "Timestamp")]
        TimestampGh,
        [Display(Name = "Global Mask")]
        GlobalMask,
        [Display(Name = "Gps Element Mask")]
        GpsElementMask,
        [Display(Name = "Longitude")]
        LongitudeGh,
        [Display(Name = "Latitude")]
        LatitudeGh,
        [Display(Name = "Angle")]
        AngleGh,
        [Display(Name = "Speed")]
        SpeedGh,
        // GpsIO elements for Codec7
        [Display(Name = "Local Area Code and Cell ID")]
        CellIdAndLocalAreaGh,
        [Display(Name = "Signal Quality")]
        SignalQualityGh,
        [Display(Name = "Operator Code")]
        OperatorCodeGh,
        // Udp types
        Length,
        [Display(Name = "Packet ID")]
        PacketId,
        [Display(Name = "Packet Type")]
        PacketType,
        [Display(Name = "AVL packet ID")]
        AvlPacketId,
        [Display(Name = "Imei length")]
        ImeiLength,
        Imei
    }
}
