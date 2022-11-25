using System;
using System.Collections.Generic;
using System.IO;
using Teltonika.Codec.Model;

namespace Teltonika.Codec.Codecs
{
	public sealed class Codec12
	{
		private const byte CodecId = 12;

		public static byte[] Encode(Command command)
		{
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(CodecId);
				writer.Write(1); // Command count
				writer.Write(command.Id);
				writer.Write(command.Data.Length);
				writer.Write(command.Data);
				writer.Write(1); // Command count
				return stream.ToArray();
			}
		}

		public static Command[] Decode(byte[] buffer)
		{
			var output = new List<Command>();
			using (var stream = new MemoryStream(buffer))
			using (var reader = new BinaryReader(stream))
			{
				var codecId = reader.ReadByte();
				if(codecId != CodecId) throw new ArgumentOutOfRangeException($"Codec id does not match, Received:{codecId}, Expected:{CodecId}");

				var count1 = reader.ReadByte();
				for (int i = 0; i < count1; i++)
				{
					var id = reader.ReadByte();
					var length = reader.ReadByte();
					var data = reader.ReadBytes(length);
					output.Add(new Command(id, data));
				}

				var count2 = reader.ReadByte();
				if(count1 != count2) throw new ArgumentOutOfRangeException($"Count does not match, Count #1:{count1}, Count #2:{count2}");
			}

			return output.ToArray();
		}
	}
}
