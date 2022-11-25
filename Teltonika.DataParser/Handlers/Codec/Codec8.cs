using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Handlers.Codec
{
    public class Codec8 : AVLCodec
    {
        public override CompositeData GetAvlData(DataReader codecReader)
        {
            CompositeData avlComposite = new CompositeData("AVL Data");
            CompositeData gpsElementComposite = new CompositeData("GPS Element");
            CompositeData ioElementComposite = new CompositeData("I/O Element");

            avlComposite.Add(codecReader.ReadData(8, DataType.Timestamp));
            avlComposite.Add(codecReader.ReadData(1, DataType.Priority));
            gpsElementComposite.Add(codecReader.ReadData(4, DataType.Longitude));
            gpsElementComposite.Add(codecReader.ReadData(4, DataType.Latitude));
            gpsElementComposite.Add(codecReader.ReadData(2, DataType.Altitude));
            gpsElementComposite.Add(codecReader.ReadData(2, DataType.Angle));
            gpsElementComposite.Add(codecReader.ReadData(1, DataType.Satellites));
            gpsElementComposite.Add(codecReader.ReadData(2, DataType.Speed));
            ioElementComposite.Add(codecReader.ReadData(1, DataType.EventIoId));
            ioElementComposite.Add(codecReader.ReadData(1, DataType.IoCount));

            var ioCount1BData = codecReader.ReadData(1, DataType.IoCount1B);
            ioElementComposite.Add(ioCount1BData);
            for (var j = 0; j < Int32.Parse(ioCount1BData.Value); j++)
            {
                ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId1B));
                ioElementComposite.Add(codecReader.ReadData(1, DataType.IoValue1B));
            }

            var ioCount2BData = codecReader.ReadData(1, DataType.IoCount2B);
            ioElementComposite.Add(ioCount2BData);
            for (var j = 0; j < Int32.Parse(ioCount2BData.Value); j++)
            {
                ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId2B));
                ioElementComposite.Add(codecReader.ReadData(2, DataType.IoValue2B));
            }

            var ioCount4BData = codecReader.ReadData(1, DataType.IoCount4B);
            ioElementComposite.Add(ioCount4BData);
            for (var j = 0; j < Int32.Parse(ioCount4BData.Value); j++)
            {
                ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId4B));
                ioElementComposite.Add(codecReader.ReadData(4, DataType.IoValue4B));
            }

            var ioCount8BData = codecReader.ReadData(1, DataType.IoCount8B);
            ioElementComposite.Add(ioCount8BData);
            for (var j = 0; j < Int32.Parse(ioCount8BData.Value); j++)
            {
                ioElementComposite.Add(codecReader.ReadData(1, DataType.IoId8B));
                ioElementComposite.Add(codecReader.ReadData(8, DataType.IoValue8B));
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
