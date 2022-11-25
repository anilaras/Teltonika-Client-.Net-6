using System;
using Teltonika.DataParser.Client.Models;

namespace Teltonika.DataParser.Client.Infrastructure
{
    public class DataReader
    {
        private readonly byte[] _data;
        private int _offset;

        public DataReader(byte[] data)
        {
            _data = data;
            _offset = 0;
        }
        public ComponentData ReadData(int size, DataType dataType)
        {
            var arraySegment = new ArraySegment<byte>(_data, _offset, size);

            _offset += size;
            return new ComponentData(dataType, arraySegment, ValueConverter.GetStringValue(arraySegment, dataType));
        }
        public ArraySegment<byte> ReadArraySeqment(int size)
        {
            _offset += size;
            return new ArraySegment<byte>(_data, _offset - size, size);
        }
    }
}
