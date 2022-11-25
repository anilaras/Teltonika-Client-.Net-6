using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.Codec.Model;
using Teltonika.Codec.Model.GH;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Handlers.Codec
{
    public class Codec7 : AVLCodec
    {
        public override CompositeData GetAvlData(DataReader codecReader)
        {
            CompositeData avlComposite = new CompositeData("AVL Data");
            CompositeData gpsElementComposite = new CompositeData("GPS Element");
            CompositeData ioElementComposite = new CompositeData("I/O Element");

            var priorityAndTimeStampSegment = codecReader.ReadArraySeqment(4);


            var timestamp = new ComponentData(DataType.TimestampGh, priorityAndTimeStampSegment, ValueConverter.GetStringValue(priorityAndTimeStampSegment, DataType.TimestampGh));
            var priority = new ComponentData(DataType.PriorityGh, priorityAndTimeStampSegment, ValueConverter.GetStringValue(priorityAndTimeStampSegment, DataType.PriorityGh));
            avlComposite.Add(timestamp);
            avlComposite.Add(priority);

            var globalMaskByteSegment = codecReader.ReadArraySeqment(1);
            var globalmask = new ComponentData(DataType.GlobalMask, globalMaskByteSegment, ValueConverter.GetStringValue(globalMaskByteSegment, DataType.GlobalMask));
            avlComposite.Add(globalmask);

            // gps Element
            if (globalmask.Value.Contains(GlobalMaskCodec7.GpsElement.ToString()))
            {
                var gpsElementMaskSegment = codecReader.ReadArraySeqment(1);
                var gpsElementMask = new ComponentData(DataType.GpsElementMask, gpsElementMaskSegment, ValueConverter.GetStringValue(gpsElementMaskSegment, DataType.GpsElementMask));
                gpsElementComposite.Add(gpsElementMask);

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Coordinates.ToString()))
                {
                    gpsElementComposite.Add(codecReader.ReadData(4, DataType.LatitudeGh));
                    gpsElementComposite.Add(codecReader.ReadData(4, DataType.LongitudeGh));
                }

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Altitude.ToString()))
                    gpsElementComposite.Add(codecReader.ReadData(2, DataType.Altitude));

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Angle.ToString()))
                    gpsElementComposite.Add(codecReader.ReadData(1, DataType.AngleGh));

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Speed.ToString()))
                    gpsElementComposite.Add(codecReader.ReadData(1, DataType.SpeedGh));

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Satellites.ToString()))
                    gpsElementComposite.Add(codecReader.ReadData(1, DataType.Satellites));

                // gps Io Elements
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.CellId.ToString()))
                    ioElementComposite.Add(codecReader.ReadData(4, DataType.CellIdAndLocalAreaGh));

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.SignalQuality.ToString()))
                    ioElementComposite.Add(codecReader.ReadData(1, DataType.SignalQualityGh));

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.OperatorCode.ToString()))
                    ioElementComposite.Add(codecReader.ReadData(4, DataType.OperatorCodeGh));

            }

            // io Elements
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt8.ToString()))
            {
                var ioCount1BData = codecReader.ReadData(1, DataType.IoCount1B);
                ioElementComposite.Add(ioCount1BData);
                for (var j = 0; j < Int32.Parse(ioCount1BData.Value); j++)
                {
                    ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId1B));
                    ioElementComposite.Add(codecReader.ReadData(1, DataType.IoValue1B));
                }

            }
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt16.ToString()))
            {
                var ioCount2BData = codecReader.ReadData(1, DataType.IoCount2B);
                ioElementComposite.Add(ioCount2BData);
                for (var j = 0; j < Int32.Parse(ioCount2BData.Value); j++)
                {
                    ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId2B));
                    ioElementComposite.Add(codecReader.ReadData(2, DataType.IoValue2B));
                }
            }
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt32.ToString()))
            {
                var ioCount4BData = codecReader.ReadData(1, DataType.IoCount4B);
                ioElementComposite.Add(ioCount4BData);
                for (var j = 0; j < Int32.Parse(ioCount4BData.Value); j++)
                {
                    ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId4B));
                    ioElementComposite.Add(codecReader.ReadData(4, DataType.IoValue4B));
                }
            }
            avlComposite.Add(gpsElementComposite);
            avlComposite.Add(ioElementComposite);

            ioElementComposite.ConfigureArraySegment();
            gpsElementComposite.ConfigureArraySegment();
            avlComposite.ConfigureArraySegment(ioElementComposite.Last().ArraySegment.Offset);

            return avlComposite;
        }
    }
}
