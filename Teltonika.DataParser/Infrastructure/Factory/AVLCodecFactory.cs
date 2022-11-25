using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.DataParser.Client.Models;
using Teltonika.DataParser.Client.Handlers.Codec;

namespace Teltonika.DataParser.Client.Infrastructure.Factory
{
    public static class AVLCodecFactory
    {
        public static AVLCodec CreateAVLCodec(string codecId)
        {
            AVLCodec codec;
            switch (codecId)
            {
                case "7":
                    codec = new Codec7();
                    break;
                case "8":
                    codec = new Codec8();
                    break;
                case "16":
                    codec = new Codec16();
                    break;
                case "142":
                    codec = new Codec8E();
                    break;
                default:
                    throw new ArgumentException();
            }
            return codec;
        }
    }
}
