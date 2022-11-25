using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Infrastructure.Interfaces
{
    public interface ICompositeData
    {
        void Add(BaseData data);
        BaseData Last();
        void ConfigureArraySegment();
    }
}
