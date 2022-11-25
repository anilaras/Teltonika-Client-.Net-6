namespace Teltonika.Codec.Model
{
	/// <summary>
	/// Codec 12 command
	/// </summary>
	public class Command
	{
		public Command(byte id, byte[] data)
		{
			Id = id;
			Data = data ?? new byte[0];
		}

		public byte Id { get; }
		public byte[] Data { get; }
	}
}
