using System.Collections.Generic;
using Teltonika.DataParser.Client.Infrastructure.Interfaces;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Infrastructure.Visitor
{
    public class GpsDataVisitor : IVisitor
    {
        private GpsData _currentGpsData = new GpsData();

        public GpsDataVisitor()
        {
            GpsData = new List<GpsData>();
        }

        public List<GpsData> GpsData { get; }

        public void Visit(BaseData componentData)
        {
            switch (componentData.Name)
            {
                case "Timestamp":
                    _currentGpsData.Timestamp = componentData.Value;
                    break;
                case "Longitude":
                    _currentGpsData.Longitude = $"{double.Parse(componentData.Value) / 10000000}";
                    break;
                case "Latitude":
                    _currentGpsData.Latitude = $"{double.Parse(componentData.Value) / 10000000}";
                    break;
            }

            CheckIsCompleted(_currentGpsData);
        }

        public void Visit(CompositeData compositeData)
        {
            foreach (var item in compositeData.Data) item.Accept(this);
        }


        private static bool DataIsCompleted(GpsData gpsDataElement)
        {
            return !string.IsNullOrEmpty(gpsDataElement.Timestamp) &&
                   !string.IsNullOrEmpty(gpsDataElement.Longitude) &&
                   !string.IsNullOrEmpty(gpsDataElement.Latitude);
        }

        private void CheckIsCompleted(GpsData gpsDataElement)
        {
            if (!DataIsCompleted(_currentGpsData)) return;

            GpsData.Add((GpsData) _currentGpsData.Clone());
            _currentGpsData = new GpsData();
        }
    }
}