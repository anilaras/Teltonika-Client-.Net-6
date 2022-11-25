using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Handlers.Codec
{
    public abstract class AVLCodec
    {
        public abstract CompositeData GetAvlData(DataReader codecReader);
    }
}
