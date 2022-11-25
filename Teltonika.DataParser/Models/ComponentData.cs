using System;
using System.Collections.Generic;
using Teltonika.DataParser.Client.Infrastructure;
using Teltonika.DataParser.Client.Infrastructure.Interfaces;

namespace Teltonika.DataParser.Client.Models
{
    public class ComponentData : BaseData
    {
        public ComponentData(DataType dataType, ArraySegment<byte> arraySegment, string value) : base(dataType, arraySegment, value) { }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
