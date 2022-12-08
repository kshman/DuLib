using Du.ModBus.Supp;

namespace Du.ModBus;

/// <summary>
/// 모드버스 기본 오브젝트
/// </summary>
[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class ModBusObject
{
	public const byte ErrorMask = 0x80;

	public const byte MinDeviceIdTcp = 0;
	public const byte MinDeviceIdRtu = 1;

	public const int MaxDeviceId = 255;

	public const int MinAddress = 0;
	public const int MaxAddress = 65535;

	public const int MinCount = 1;

	public const int MaxCoilCountRead = 2000;
	public const int MaxCoilCountWrite = 1968;

	public const int MaxRegisterCountRead = 125;
	public const int MaxRegisterCountWrite = 123;

	public virtual ModBusType Type { get; set; }

	public int Address { get; set; }

	public byte ByteHi { get; set; }

	public byte ByteLo { get; set; }

	public ushort Register
	{
		get
		{
			var bs = new[] { ByteHi, ByteLo };
			return BitConverter.ToUInt16(bs.TestLittleEndian(), 0);
		}
		set
		{
			var bs = BitConverter.GetBytes(value).TestLittleEndian();
			ByteHi = bs[0];
			ByteLo = bs[1];
		}
	}

	public bool AsBool
	{
		get => ByteHi > 0 || ByteLo > 0;
		set
		{
			ByteHi = 0;
			ByteLo = (byte)(value ? 1 : 0);
		}
	}

	public override string? ToString()
		=> Type switch
		{
			ModBusType.Coil => $"{Cpr.coil}: {Address} ➜ {AsBool}",
			ModBusType.DiscreteInput => $"{Cpr.discrete}: {Address} ➜ {AsBool}",
			ModBusType.HoldingRegister => $"{Cpr.holding}: {Address} ➜ {ByteHi:X02} {ByteLo:X02} | {Register}",
			ModBusType.InputRegister => $"{Cpr.input}: {Address} ➜ {ByteHi:X02} {ByteLo:X02} | {Register}",
			_ => base.ToString(),
		};

	public override bool Equals(object? obj)
	{
		if (obj is not ModBusObject o)
			return false;
		return
			Type == o.Type &&
			Address == o.Address &&
			ByteHi == o.ByteHi &&
			ByteLo == o.ByteLo;
	}

	public override int GetHashCode()
		=> base.GetHashCode() ^ Address.GetHashCode() ^ ByteHi.GetHashCode() ^ ByteLo.GetHashCode();
}

/// <summary>
/// 모드버스 Coil
/// </summary>
public class ModBusCoil : ModBusObject
{
	public override ModBusType Type => ModBusType.Coil;
}

/// <summary>
/// 모드버스 Discrete Input
/// </summary>
public class ModBusDiscreteInput : ModBusObject
{
	public override ModBusType Type => ModBusType.DiscreteInput;
}

/// <summary>
/// 모드버스 Register
/// </summary>
public class ModBusRegister : ModBusObject
{
	#region 모드 지정 만들기
	public static ModBusObject New(byte value, int address, ModBusType type)
		=> new ModBusRegister()
		{
			Type = type,
			Address = address,
			Register = value,
		};

	public static ModBusObject New(ushort value, int address, ModBusType type)
		=> new ModBusRegister()
		{
			Type = type,
			Address = address,
			Register = value,
		};

	public static List<ModBusObject> New(uint value, int address, ModBusType type)
	{
		if (address + 1 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static List<ModBusObject> New(ulong value, int address, ModBusType type)
	{
		if (address + 3 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static ModBusObject New(sbyte value, int address, ModBusType type)
		=> new ModBusRegister()
		{
			Type = type,
			Address = address,
			Register = (ushort)value
		};

	public static ModBusObject New(short value, int address, ModBusType type)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		return new ModBusRegister
		{
			Type = type,
			Address = address,
			ByteHi = bs[0],
			ByteLo = bs[1]
		};
	}

	public static List<ModBusObject> New(int value, int address, ModBusType type)
	{
		if (address + 1 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static List<ModBusObject> New(long value, int address, ModBusType type)
	{
		if (address + 3 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static List<ModBusObject> New(float value, int address, ModBusType type)
	{
		if (address + 1 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static List<ModBusObject> New(double value, int address, ModBusType type)
	{
		if (address + 3 > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var os = new List<ModBusObject>();

		for (var i = 0; i < bs.Length / 2; i++)
		{
			var p = i * 2;
			os.Add(new ModBusRegister()
			{
				Type = type,
				Address = address + i,
				ByteHi = bs[p],
				ByteLo = bs[p + 1]
			});
		}

		return os;
	}

	public static List<ModBusObject> New(string str, int address, Encoding encoding, ModBusType type)
	{
		var bs = encoding.GetBytes(str);
		var len = (int)Math.Ceiling(bs.Length / 2.0);

		if (address + len > MaxAddress)
			throw new ArgumentOutOfRangeException(nameof(address));

		var os = new List<ModBusObject>();

		for (var i = 0; i < len; i++)
		{
			var p = i * 2;
			try
			{
				os.Add(new ModBusRegister()
				{
					Type = type,
					Address = address + i,
					ByteHi = bs[p],
					ByteLo = bs[p + 1]
				});
			}
			catch
			{
				os.Add(new ModBusRegister()
				{
					Type = type,
					Address = address + i,
					ByteHi = bs[p]
				});
			}
		}

		return os;
	}

	public static List<ModBusObject> New(string str, int address, ModBusType type)
		=> New(str, address, ModBusConfig.DefaultEncoding, type);
	#endregion

	#region 기본 모드로 만들기
	public static ModBusObject New(byte value, int address)
	=> New(value, address, ModBusConfig.DefaultType);

	public static ModBusObject New(ushort value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(uint value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(ulong value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static ModBusObject New(sbyte value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static ModBusObject New(short value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(int value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(long value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(float value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(double value, int address)
		=> New(value, address, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(string str, int address, Encoding encoding)
		=> New(str, address, encoding, ModBusConfig.DefaultType);

	public static List<ModBusObject> New(string str, int address) =>
		New(str, address, ModBusConfig.DefaultEncoding, ModBusConfig.DefaultType);
	#endregion
}
