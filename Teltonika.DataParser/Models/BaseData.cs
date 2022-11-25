using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Teltonika.Codec;
using Teltonika.DataParser.Client.Infrastructure.Interfaces;
using DataType = Teltonika.DataParser.Client.Infrastructure.DataType;

namespace Teltonika.DataParser.Client.Models
{
    public abstract class BaseData : IComponent
    {
        public BaseData(string name)
        {
            _name = name;
        }

        public BaseData(DataType dataType, ArraySegment<byte> arraySegment, string value)
        {
            DataType = dataType;
            Value = value;
            ArraySegment = arraySegment;
            BitValues = GetBitValues(arraySegment, dataType);
        }

        public DataType DataType { get; set; }
        public ArraySegment<byte> ArraySegment { get; set; }
        public string Value { get; set; }
        public BitValue BitValues { get; set; }
        public string HexValue
        {
            get
            {
                if (_name == null)
                {
                    return ArraySegment.Array != null
                        ? BitConverter.ToString(ArraySegment.Array.Skip(ArraySegment.Offset).Take(ArraySegment.Count).ToArray())
                        : null;
                }
                return null;
            }
        }

        public string Size
        {
            get
            {
                if (_name == "GPS Element")
                {
                    return ArraySegment.Count.ToString(CultureInfo.InvariantCulture);
                }
                if (ArraySegment.Count == 0 || _name != null)
                {
                    return "var";
                }

                return ArraySegment.Count.ToString(CultureInfo.InvariantCulture);
            }
        }

        private readonly string _name;
        public string Name
        {
            get
            {
                return _name ?? GetDataTypeDisplayName();
            }
        }

        private string GetDataTypeDisplayName()
        {
            var type = DataType.GetType();

            var members = type.GetMember(DataType.ToString());
            if (members.Length == 0) return DataType.ToString();

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) return DataType.ToString();

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }

        private BitValue GetBitValues(ArraySegment<byte> arraySegment, DataType dataType)
        {
            var result = new BitValue();

            if (dataType == DataType.PriorityGh || dataType == DataType.TimestampGh)
            {
                var bits = BitConverters.ByteArrayToBits(arraySegment.ToArray());

                result = new BitValue
                {
                    FirstPartBits = bits.Substring(0, 2),
                    SecondPartBits = bits.Substring(2, bits.Length - 2),
                    FirstPartBitsTextColor = dataType == DataType.PriorityGh ? "#FFFF0000" : "#00000000",
                    SecondPartBitsTextColor = dataType == DataType.PriorityGh ? "#00000000" : "#FFFF0000"
                };
            }
            return result;
        }

        public abstract void Accept(IVisitor visitor);
    }
}
