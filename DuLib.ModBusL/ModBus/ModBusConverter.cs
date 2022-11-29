using Du.ModBus.Supp;

namespace Du.ModBus;

/// <summary>
/// 모드버스 타입 변환기
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public static class ModBusConverter
{
	#region 기본 타입으로 변환
	private static void InternalCheckType(ModBusObject obj)
	{
		if (obj.Type != ModBusType.HoldingRegister &&
			obj.Type != ModBusType.InputRegister)
			throw new ArgumentException(Cpr.ex_invalid_register_type);
	}

	private static void InternalCheckRegister(IEnumerable<ModBusObject> objs, int startIndex, int takeRegisters)
	{
		var count = objs.Count();
		if (count < takeRegisters)
			throw new ArgumentException(Cpr.ex_need_reg_at_least + takeRegisters, nameof(objs));

		if (startIndex < 0 || count < startIndex + takeRegisters)
			throw new ArgumentOutOfRangeException(nameof(startIndex));

		if (objs.Any(r => r.Type != ModBusType.HoldingRegister) &&
			objs.Any(r => r.Type != ModBusType.InputRegister))
			throw new ArgumentException(Cpr.ex_invalid_register_type);
	}

	private static byte[] InternalTakeRegister(IEnumerable<ModBusObject>? objs, int startIndex, int takeRegisters, bool inverseRegisters)
	{
		if (objs == null)
			throw new ArgumentNullException(nameof(objs));

		InternalCheckRegister(objs, startIndex, takeRegisters);

		var os = objs
			.OrderBy(r => r.Address)
			.Skip(startIndex)
			.Take(takeRegisters);
		var rs = inverseRegisters ? os.Reverse().ToArray() : os.ToArray();
		var bs = new byte[rs.Length * 2];
		for (var i = 0; i < rs.Length; i++)
		{
			bs[i * 2] = rs[i].ByteHi;
			bs[i * 2 + 1] = rs[i].ByteLo;
		}

		return bs.TestLittleEndian();
	}

	public static bool GetBool(this ModBusObject? obj)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		return obj.Register > 0;
	}

	public static byte GetByte(this ModBusObject? obj)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		InternalCheckType(obj);
		return (byte)obj.Register;
	}

	public static ushort GetUInt16(this ModBusObject? obj)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		InternalCheckType(obj);
		return obj.Register;
	}

	public static uint GetUInt32(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 2, inverseRegisters);
		return BitConverter.ToUInt32(bs, 0);
	}

	public static ulong GetUInt64(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 4, inverseRegisters);
		return BitConverter.ToUInt64(bs, 0);
	}

	public static sbyte GetSByte(this ModBusObject? obj)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		InternalCheckType(obj);
		return (sbyte)obj.Register;
	}

	public static short GetInt16(this ModBusObject? obj)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		InternalCheckType(obj);
		return BitConverter.ToInt16((new[] { obj.ByteHi, obj.ByteLo }).TestLittleEndian(), 0);
	}

	public static int GetInt32(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 2, inverseRegisters);
		return BitConverter.ToInt32(bs, 0);
	}

	public static long GetInt64(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 4, inverseRegisters);
		return BitConverter.ToInt64(bs, 0);
	}

	public static float GetSingle(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 2, inverseRegisters);
		return BitConverter.ToSingle(bs, 0);
	}

	public static double GetDouble(this IEnumerable<ModBusObject>? objs, int startIndex = 0, bool inverseRegisters = false)
	{
		var bs = InternalTakeRegister(objs, startIndex, 4, inverseRegisters);
		return BitConverter.ToDouble(bs, 0);
	}

	private static readonly char[] s_trim_chars = new[] { ' ', '\t', '\0', '\r', '\n' };

	public static string GetString(this IEnumerable<ModBusObject>? objs, Encoding encoding, int length, int startIndex, bool flipBytes = false)
	{
		if (objs == null)
			throw new ArgumentNullException(nameof(objs));

		InternalCheckRegister(objs, startIndex, length);

		var rs = objs
			.Skip(startIndex)
			.Take(length)
			.ToArray();
		var bs = new byte[rs.Length * 2];
		if (flipBytes)
		{
			for (var i = 0; i < rs.Length; i++)
			{
				bs[i * 2] = rs[i].ByteHi;
				bs[i * 2 + 1] = rs[i].ByteLo;
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				bs[i * 2] = rs[i].ByteLo;
				bs[i * 2 + 1] = rs[i].ByteHi;
			}
		}

		var s = encoding.GetString(bs).Trim(s_trim_chars);
		var n = s.IndexOf('\0');
		return n >= 0 ? s[..n] : s;
	}

	public static string GetString(this IEnumerable<ModBusObject>? objs, Encoding encoding, int length,
		bool flipBytes = false)
		=> GetString(objs, encoding, length, 0, flipBytes);

	public static string GetString(this IEnumerable<ModBusObject>? objs, int length, int startIndex, bool flipBytes = false)
		=> GetString(objs, ModBusConfig.DefaultEncoding, length, startIndex, flipBytes);

	public static string GetString(this IEnumerable<ModBusObject>? objs, int length, bool flipBytes = false)
		=> GetString(objs, ModBusConfig.DefaultEncoding, length, 0, flipBytes);
	#endregion

	#region 모드버스로 변환
	public static ModBusCoil ToModBusCoil(this bool value, int address)
		=> new ModBusCoil()
		{
			Address = address,
			AsBool = value,
		};
	public static ModBusObject ToModBusRegister(this bool value, int address)
		=> new ModBusRegister()
		{
			Type = ModBusType.HoldingRegister,
			Address = address,
			Register = (ushort)(value ? 1 : 0)
		};

	public static ModBusObject ToModBusRegister(this byte value, int address)
		=> new ModBusRegister()
		{
			Type = ModBusType.HoldingRegister,
			Address = address,
			Register = value
		};

	public static ModBusObject ToModBusRegister(this ushort value, int address)
		=> new ModBusRegister()
		{
			Type = ModBusType.HoldingRegister,
			Address = address,
			Register = value
		};

	public static ModBusObject[] ToModBusRegister(this uint value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this ulong value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)

			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject ToModBusRegister(this sbyte value, int address)
		=> new ModBusRegister()
		{
			Type = ModBusType.HoldingRegister,
			Address = address,
			Register = (ushort)value
		};

	public static ModBusObject ToModBusRegister(this short value, int address)
		=> new ModBusRegister()
		{
			Type = ModBusType.HoldingRegister,
			Address = address,
			Register = (ushort)value
		};

	public static ModBusObject[] ToModBusRegister(this int value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this long value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this float value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this double value, int address, bool inverseRegisters = false)
	{
		var bs = BitConverter.GetBytes(value).TestLittleEndian();
		var rs = new ModBusObject[bs.Length / 2];

		if (inverseRegisters)
		{
			var sa = address + rs.Length - 1;
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = (ushort)(sa - i),
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = bs[i * 2],
					ByteLo = bs[i * 2 + 1]
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this string? str, Encoding encoding, int address, int length, bool flipBytes = false)
	{
		if (str == null)
			throw new ArgumentNullException(nameof(str));

		var bs = encoding.GetBytes(str);
		if (length > 0) Array.Resize(ref bs, length);
		var rs = new ModBusObject[(int)Math.Ceiling(bs.Length / 2.0)];

		if (flipBytes)
		{
			for (var i = 0; i < rs.Length; i++)
			{
				var hi = i * 2 + 1 < bs.Length ? bs[i * 2 + 1] : (byte)0;
				var lo = bs[i * 2];

				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = hi,
					ByteLo = lo
				};
			}
		}
		else
		{
			for (var i = 0; i < rs.Length; i++)
			{
				var hi = bs[i * 2];
				var lo = i * 2 + 1 < bs.Length ? bs[i * 2 + 1] : (byte)0;

				rs[i] = new ModBusRegister
				{
					Type = ModBusType.HoldingRegister,
					Address = address + i,
					ByteHi = hi,
					ByteLo = lo
				};
			}
		}

		return rs;
	}

	public static ModBusObject[] ToModBusRegister(this string? str, Encoding encoding, int address, 
		bool flipBytes = false)
		=> ToModBusRegister(str, encoding, address, 0, flipBytes);


	public static ModBusObject[] ToModBusRegister(this string? str, int address, int length, bool flipBytes = false)
		=> ToModBusRegister(str, ModBusConfig.DefaultEncoding, address, length, flipBytes);

	public static ModBusObject[] ToModBusRegister(this string? str, int address, bool flipBytes = false)
		=> ToModBusRegister(str, ModBusConfig.DefaultEncoding, address, 0, flipBytes);
	#endregion
}
