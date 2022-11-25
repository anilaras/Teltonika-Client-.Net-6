using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Teltonika.DataParser.Client.Handlers;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Models;
using System.Globalization;
using System.Text.Json;
using Teltonika.DataParser.Client.Infrastructure.Visitor;

namespace TcpListener
{
    public class DecodeDataService
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        string text; 
        bool? packetType = true;//true == TCP ; false == UDP
        private IList<GpsData> _gpsData;
        public DecodeDataService(string text, bool? packetType = true)
        {
            this.packetType = packetType;
            this.text = text;
        }
        public async Task Run() 
        {
            Log.Info("Decoding Data ");
            try
            {
                var bytes = StringToBytes(text);
                var reader = new DataReader(bytes);
                var packetDecoder = new PacketDecoder();


                CompositeData data;
                if (packetType != null && (bool)packetType)
                    data = packetDecoder.DecodeTCPData(reader);
                else
                    data = packetDecoder.DecodeUdpData(reader);
                HandleAvl(data);
                HandleGps(data);
            }
            catch
            {

            }

        }

        private void HandleGps(CompositeData data)
        {
            var gpsDataVisitor = new GpsDataVisitor();
            data.Accept(gpsDataVisitor);
            _gpsData = gpsDataVisitor.GpsData;
            Log.Info(JsonSerializer.Serialize(_gpsData));

            //gpsElementsListView.ItemsSource = _gpsData;
            //_markersHandler.LoadMarkers(_gpsData);
        }

        private void HandleAvl(CompositeData data)
        {
            var listViewVisitor = new TransposedAvlDataVisitor();
            data.Accept(listViewVisitor);
            var dataTable = listViewVisitor.DataTable;
            //AvlTableDataGrid.DataContext = dataTable.DefaultView;
        }

        private static byte[] StringToBytes(string data)
        {
            var array = new byte[data.Length / 2];

            var substring = 0;
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = byte.Parse(data.Substring(substring, 2), NumberStyles.AllowHexSpecifier);
                substring += 2;
            }

            return array;
        }
    }
}
