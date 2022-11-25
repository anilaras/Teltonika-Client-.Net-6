using System;
using GMap.NET;

namespace Teltonika.DataParser.Client.Models
{
    public class GpsData : ICloneable
    {
        public string Timestamp { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public PointLatLng Coordinates { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}