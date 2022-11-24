namespace Du.ModBus.Protocol;

/// <summary>
/// 요청
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class Request
{
    public ushort TransactionId { get; set; }
    public byte DeviceId { get; set; }
    public ModBusFunctionCode Function { get; set; }
    public int Address { get; set; }
    public int Count { get; set; }

    public DataBuffer? Data { get; set; }
    public byte[] Bytes
    {
        get => Data?.Buffer ?? Array.Empty<byte>();
        set => Data = new DataBuffer(value);
    }

    public ModBusMei Mei { get; set; }

    public ModBusDevIdCategory MeiCategory { get; set; }

    public ModBusDevIdObject MeiObject { get; set; }

    public Request()
    { }

    public Request(IEnumerable<byte> bytes)
    {
        Deserialize(bytes);
    }

    public byte[] Serialize()
    {
        var buffer = new DataBuffer(8);

        buffer.SetUInt16(0, TransactionId);
        buffer.SetUInt16(2, 0x0000); // Protocol ID

        buffer.SetByte(6, DeviceId);
        buffer.SetByte(7, (byte)Function);

        switch (Function)
        {
            case ModBusFunctionCode.ReadCoils:
            case ModBusFunctionCode.ReadDiscreteInputs:
            case ModBusFunctionCode.ReadHoldingRegisters:
            case ModBusFunctionCode.ReadInputRegisters:
                buffer.AddUInt16((ushort)Address);
                buffer.AddUInt16((ushort)Count);
                break;

            case ModBusFunctionCode.WriteMultipleCoils:
            case ModBusFunctionCode.WriteMultipleRegisters:
                buffer.AddUInt16((ushort)Address);
                buffer.AddUInt16((ushort)Count);
                buffer.AddByte((byte)(Data?.Length ?? 0));
                if (Data?.Length > 0)
                    buffer.AddBytes(Data.Buffer);
                break;

            case ModBusFunctionCode.WriteSingleCoil:
            case ModBusFunctionCode.WriteSingleRegister:
                buffer.AddUInt16((ushort)Address);
                if (Data?.Length > 0)
                    buffer.AddBytes(Data.Buffer);
                break;

            case ModBusFunctionCode.EncapsulatedInterface:
                buffer.AddByte((byte)Mei);
                switch (Mei)
                {
                    case ModBusMei.CanOpenGeneralReference:
                        if (Data?.Length > 0)
                            buffer.AddBytes(Data.Buffer);
                        break;

                    case ModBusMei.ReadDeviceInformation:
                        buffer.AddByte((byte)MeiCategory);
                        buffer.AddByte((byte)MeiObject);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                break;
            default:
                throw new NotImplementedException();
        }

        var len = buffer.Length - 6;
        buffer.SetUInt16(4, (ushort)len);

        return buffer.Buffer;
    }

    private void Deserialize(IEnumerable<byte> bytes)
    {
        var buffer = new DataBuffer(bytes);

        TransactionId = buffer.GetUInt16(0);
        var ident = buffer.GetUInt16(2);
        if (ident != 0)
            throw new ArgumentException(Cpr.ex_invalid_protocol_identifier);

        var length = buffer.GetUInt16(4);
        if (buffer.Length < length + 6)
            throw new ArgumentException(Cpr.ex_data_length_less);

        if (buffer.Length > length + 6)
        {
            if (buffer.Buffer.Skip(length + 6).Any(b => b != 0))
                throw new ArgumentException(Cpr.ex_data_length_many);

            buffer = new DataBuffer(bytes.Take(length + 6));
        }

        DeviceId = buffer.GetByte(6);
        Function = (ModBusFunctionCode)buffer.GetByte(7);

        switch (Function)
        {
            case ModBusFunctionCode.ReadCoils:
            case ModBusFunctionCode.ReadDiscreteInputs:
            case ModBusFunctionCode.ReadHoldingRegisters:
            case ModBusFunctionCode.ReadInputRegisters:
                Address = buffer.GetUInt16(8);
                Count = buffer.GetUInt16(10);
                break;

            case ModBusFunctionCode.WriteMultipleCoils:
            case ModBusFunctionCode.WriteMultipleRegisters:
                Address = buffer.GetUInt16(8);
                Count = buffer.GetUInt16(10);
                Data = new DataBuffer(buffer.Buffer.Skip(12));
                break;

            case ModBusFunctionCode.WriteSingleCoil:
            case ModBusFunctionCode.WriteSingleRegister:
                Address = buffer.GetUInt16(8);
                Data = new DataBuffer(buffer.Buffer.Skip(10));
                break;

            case ModBusFunctionCode.EncapsulatedInterface:
                Mei = (ModBusMei)buffer.GetByte(8);
                switch (Mei)
                {
                    case ModBusMei.CanOpenGeneralReference:
                        Data = new DataBuffer(buffer.Buffer.Skip(9));
                        break;

                    case ModBusMei.ReadDeviceInformation:
                        MeiCategory = (ModBusDevIdCategory)buffer.GetByte(9);
                        MeiObject = (ModBusDevIdObject)buffer.GetByte(10);
                        break;

                    default:
                        throw new NotImplementedException(Cpr.ex_unk_mei_with + Mei);
                }
                break;
            default:
                throw new NotImplementedException(Cpr.ex_unk_func_with + Function);
        }
    }

    public override string ToString()
        => $"Request#{TransactionId} (#{DeviceId}, @{Function}, {Address}[{Count}]) {string.Join(" ", Bytes.Select(b => b.ToString("X2")))}";

    public override int GetHashCode()
        => base.GetHashCode() ^
            TransactionId.GetHashCode() ^
            DeviceId.GetHashCode() ^
            Function.GetHashCode() ^
            Address.GetHashCode() ^
            Count.GetHashCode() ^
            Bytes.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not Request req)
            return false;
        if (Data == null)
            return false;
        return req.TransactionId == TransactionId &&
            req.DeviceId == DeviceId &&
            req.Function == Function &&
            req.Address == Address &&
            req.Count == Count &&
            Data.Equals(req.Data);
    }

}
