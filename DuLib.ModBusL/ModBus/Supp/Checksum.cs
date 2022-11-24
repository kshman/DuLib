namespace Du.ModBus.Supp;

public static class Checksum
{
	private static byte[] InternalCrc16(IReadOnlyList<byte> array, int start, int length)
	{
		ushort crc16 = 0xFFFF;

		for (var i = start; i < start + length; i++)
		{
			crc16 = (ushort)(crc16 ^ array[i]);
			for (var j = 0; j < 8; j++)
			{
				var lsb = (byte)(crc16 & 1);
				crc16 = (ushort)(crc16 >> 1);
				if (lsb == 1)
					crc16 = (ushort)(crc16 ^ 0xA001);
			}
		}

		var b = new[]
		{
			(byte)(crc16 & 0xFF),
			(byte)(crc16 >> 8),
		};

		return b;
	}

	public static byte[] Crc16(this byte[]? array, int start, int length)
	{
		if (array == null || array.Length == 0)
			throw new ArgumentNullException(nameof(array));

		if (start < 0 || start >= array.Length)
			throw new ArgumentOutOfRangeException(nameof(start));

		if (length <= 0 || start + length > array.Length)
			throw new ArgumentOutOfRangeException(nameof(length));

		return InternalCrc16(array, start, length);
	}

	public static byte[] Crc16(this byte[]? array)
	{
		if (array == null || array.Length == 0)
			throw new ArgumentNullException(nameof(array));

		return InternalCrc16(array, 0, array.Length);
	}
}
