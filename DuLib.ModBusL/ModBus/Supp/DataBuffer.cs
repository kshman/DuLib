namespace Du.ModBus.Supp;

/// <summary>
/// 데이터 버퍼
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
public class DataBuffer
{
    internal static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public byte[] Buffer { get; private set; }
    public int Length => Buffer.Length;

    public bool IsLittleEndian { get; set; }

    public byte this[int index]
    {
        get => GetByte(index);
        set => SetByte(index, value);
    }

    public DataBuffer()
    {
        Buffer = Array.Empty<byte>();
    }

    public DataBuffer(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        Buffer = new byte[count];
    }

    public DataBuffer(IEnumerable<byte> bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));

        Buffer = bytes as byte[] ?? bytes.ToArray();
    }

    public DataBuffer(DataBuffer buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        Buffer = new byte[buffer.Length];
        Array.Copy(buffer.Buffer, Buffer, Length);
    }

    public void SetBytes(int index, byte[] bytes)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (Length < index + bytes.Length)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        Array.Copy(bytes, 0, Buffer, index, bytes.Length);
    }

    public void SetBytes(int index, DataBuffer block)
        => SetBytes(index, block.Buffer);

    public void SetByte(int index, byte value)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        Buffer[index] = value;
    }

    public void SetBoolean(int index, bool value)
        => SetByte(index, (byte)(value ? 1 : 0));

    public void SetUInt16(int index, ushort value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetUInt32(int index, uint value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetUInt64(int index, ulong value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetSByte(int index, sbyte value)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        Buffer[index] = Convert.ToByte(value);
    }

    public void SetInt16(int index, short value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetInt32(int index, int value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetInt64(int index, long value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetSingle(int index, float value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        SetBytes(index, bs);
    }

    public void SetDouble(int index, double value)
        => SetInt64(index, BitConverter.DoubleToInt64Bits(value));

    public void SetTimeSpan(int index, TimeSpan value)
        => SetInt64(index, value.Ticks);

    public void SetDateTime(int index, DateTime value)
    {
        var ts = value.Subtract(Globalization.UnixTime.Epoch);
        SetTimeSpan(index, ts);
    }

    public void SetChar(int index, char value)
        => SetByte(index, Convert.ToByte(value));

    public int SetString(int index, string value, Encoding encoding)
    {
        var bs = encoding.GetBytes(value);
        SetBytes(index, bs);
        return bs.Length;
    }

    public int SetString(int index, string value)
        => SetString(index, value, ModBusConfig.DefaultEncoding);

    public void AddBytes(byte[] bytes)
    {
        var newBytes = new byte[Length + bytes.Length];
        Array.Copy(Buffer, 0, newBytes, 0, Length);
        Array.Copy(bytes, 0, newBytes, Length, bytes.Length);

        Buffer = newBytes;
    }

    public void AddByte(byte value)
        => AddBytes(new[] { value });

    public void AddUint8(byte value)
        => AddBytes(new[] { value });

    public void AddBoolean(bool value)
        => AddByte((byte)(value ? 1 : 0));

    public void AddUInt16(ushort value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddUInt32(uint value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddUInt64(ulong value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddSByte(sbyte value)
        => AddBytes(new[] { Convert.ToByte(value) });

    public void AddInt16(short value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddInt32(int value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddInt64(long value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddSingle(float value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddDouble(double value)
    {
        var bs = InternalSwapBytes(BitConverter.GetBytes(value));
        AddBytes(bs);
    }

    public void AddTimeSpan(TimeSpan value)
        => AddInt64(value.Ticks);

    public void AddDateTime(DateTime value)
    {
        var dt = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
        var ts = dt.Subtract(Globalization.UnixTime.BaseDateTime);
        AddTimeSpan(ts);
    }

    public void AddChar(char value)
        => AddByte(Convert.ToByte(value));

    public int AddString(string value, Encoding encoding)
    {
        var bs = encoding.GetBytes(value);
        AddBytes(bs);
        return bs.Length;
    }

    public int AddString(string value)
        => AddString(value, ModBusConfig.DefaultEncoding);

    public byte[] GetBytes(int index, int count)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (Length < index + count)
            throw new ArgumentOutOfRangeException(nameof(count));

        var bytes = new byte[count];
        Array.Copy(Buffer, index, bytes, 0, count);

        return bytes;
    }

    public byte GetByte(int index)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Buffer[index];
    }

    public bool GetBoolean(int index)
    {
        return GetByte(index) > 0;
    }

    public ushort GetUInt16(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 2));
        return BitConverter.ToUInt16(bs, 0);
    }

    public uint GetUInt32(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 4));
        return BitConverter.ToUInt32(bs, 0);
    }

    public ulong GetUInt64(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 8));
        return BitConverter.ToUInt64(bs, 0);
    }

    public sbyte GetSByte(int index)
    {
        if (index < 0 || Length <= index)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Convert.ToSByte(Buffer[index]);
    }

    public short GetInt16(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 2));
        return BitConverter.ToInt16(bs, 0);
    }

    public int GetInt32(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 4));
        return BitConverter.ToInt32(bs, 0);
    }

    public long GetInt64(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 8));
        return BitConverter.ToInt64(bs, 0);
    }

    public float GetSingle(int index)
    {
        var bs = InternalSwapBytes(GetBytes(index, 4));
        return BitConverter.ToSingle(bs, 0);
    }

    public double GetDouble(int index)
        => BitConverter.Int64BitsToDouble(GetInt64(index));

    public TimeSpan GetTimeSpan(int index)
    {
        return TimeSpan.FromTicks(GetInt64(index));
    }

    public DateTime GetDateTime(int index)
    {
        var ts = GetTimeSpan(index);
        var dt = Globalization.UnixTime.BaseDateTime.Add(ts);
        return dt;
    }

    public char GetChar(int index)
        => Convert.ToChar(GetByte(index));

    public string GetString(int index, int count, Encoding encoding)
    {
        var bs = GetBytes(index, count);
        return encoding.GetString(bs);
    }

    public string GetString(int index, int count)
        => GetString(index, count, ModBusConfig.DefaultEncoding);

    public bool IsEqual(int index, byte[] bytes)
    {
        if (Length < index + bytes.Length)
            return false;

        return !bytes.Where((t, i) => Buffer[index + i] != t).Any();
    }

    public void ResizeTo(int size)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size));

        var bs = new byte[size];
        var len = Math.Min(size, Length);
        Array.Copy(Buffer, bs, len);
        Buffer = bs;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not DataBuffer block)
            return false;
        if (block.IsLittleEndian != IsLittleEndian)
            return false;
        if (block.Length != Length)
            return false;
        return IsEqual(0, block.Buffer);
    }

    public override int GetHashCode()
        => base.GetHashCode() ^ Length.GetHashCode() ^ Buffer.GetHashCode() ^ IsLittleEndian.GetHashCode();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"DataBuffer ({Length} 바이트");
        if (IsLittleEndian) sb.Append(", 리틀엔디안");
        sb.Append(')');
        sb.AppendLine();

        sb.Append("         0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F");
        for (var i = 0; i < Length; i++)
        {
            if (i % 16 == 0)
            {
                sb.AppendLine();
                sb.Append("0x" + i.ToString("X4"));
                sb.Append(' ');
            }
            var hex = Buffer[i].ToString("X2");
            sb.Append($" {hex}");
        }
        sb.AppendLine();

        return sb.ToString();
    }

    private byte[] InternalSwapBytes(byte[] bytes)
    {
        return IsLittleEndian != BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
    }
}
