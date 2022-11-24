namespace Du.ModBus.Protocol;

/// <summary>
/// 응답
/// </summary>
[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
public class Response
{
    public ushort TransactionId { get; private set; }
    public byte DeviceId { get; private set; }
    public ModBusFunctionCode Function { get; private set; }

    public bool IsError => ErrorCode > 0;
    public ModBusErrorCode ErrorCode { get; set; }
    public string ErrorMessage => ErrorCode.GetDescription();

    public int Address { get; set; }
    public int Count { get; set; }

    public DataBuffer Data { get; set; }

    public bool IsTimeout { get; private set; }

    public ModBusMei Mei { get; set; }
    public ModBusDevIdCategory MeiCategory { get; set; }
    public ModBusDevIdObject MeiObject { get; set; }

    public byte ConformityLevel { get; set; }
    public bool MoreRequestsNeeded { get; set; }
    public byte NextObjectId { get; set; }
    public byte ObjectCount { get; set; }

    public Response(Request request)
    {
        TransactionId = request.TransactionId;
        DeviceId = request.DeviceId;
        Function = request.Function;
        Address = request.Address;
        Count = request.Count;
        Data = new DataBuffer();
    }

#pragma warning disable CS8618
    public Response(byte[] response)
    {
        Deserialize(response);
    }
#pragma warning restore CS8618

    public byte[] Serialize()
    {
        var buffer = new DataBuffer(8);

        buffer.SetUInt16(0, TransactionId);
        buffer.SetUInt16(2, 0x0000);
        buffer.SetByte(6, DeviceId);

        var fn = (byte)Function;
        if (IsError)
        {
            fn = (byte)(fn | ModBusObject.ErrorMask);
            buffer.AddByte((byte)ErrorCode);
        }
        else
        {
            switch (Function)
            {
                case ModBusFunctionCode.ReadCoils:
                case ModBusFunctionCode.ReadDiscreteInputs:
                case ModBusFunctionCode.ReadHoldingRegisters:
                case ModBusFunctionCode.ReadInputRegisters:
                    if (Data == null)
                        throw new ArgumentException(Cpr.ex_no_data);
                    buffer.AddByte((byte)Data.Length);
                    buffer.AddBytes(Data.Buffer);
                    break;

                case ModBusFunctionCode.WriteMultipleCoils:
                case ModBusFunctionCode.WriteMultipleRegisters:
                    buffer.AddUInt16((ushort)Address);
                    buffer.AddUInt16((ushort)Count);
                    break;

                case ModBusFunctionCode.WriteSingleCoil:
                case ModBusFunctionCode.WriteSingleRegister:
                    if (Data == null)
                        throw new ArgumentException(Cpr.ex_no_data);
                    buffer.AddUInt16((ushort)Address);
                    buffer.AddBytes(Data.Buffer);
                    break;

                case ModBusFunctionCode.EncapsulatedInterface:
                    buffer.AddByte((byte)Mei);
                    switch (Mei)
                    {
                        case ModBusMei.CanOpenGeneralReference:
                            if (Data.Length > 0)
                                buffer.AddBytes(Data.Buffer);
                            break;

                        case ModBusMei.ReadDeviceInformation:
                            if (Data == null)
                                throw new ArgumentException(Cpr.ex_no_data);
                            buffer.AddByte((byte)MeiCategory);
                            buffer.AddByte(ConformityLevel);
                            buffer.AddByte((byte)(MoreRequestsNeeded ? 0xFF : 0x00));
                            buffer.AddByte(NextObjectId);
                            buffer.AddByte(ObjectCount);
                            buffer.AddBytes(Data.Buffer);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        buffer.SetByte(7, fn);

        var len = buffer.Length - 6;
        buffer.SetUInt16(4, (ushort)len);

        return buffer.Buffer;
    }

    private void Deserialize(byte[] bytes)
    {
        // Response timed out => device not available
        if (bytes.All(b => b == 0))
        {
            IsTimeout = true;
            return;
        }

        var buffer = new DataBuffer(bytes);
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

        TransactionId = buffer.GetUInt16(0);
        DeviceId = buffer.GetByte(6);

        var fn = buffer.GetByte(7);
        if ((fn & ModBusObject.ErrorMask) > 0)
        {
            Function = (ModBusFunctionCode)(fn ^ ModBusObject.ErrorMask);
            ErrorCode = (ModBusErrorCode)buffer.GetByte(8);
        }
        else
        {
            Function = (ModBusFunctionCode)fn;

            switch (Function)
            {
                case ModBusFunctionCode.ReadCoils:
                case ModBusFunctionCode.ReadDiscreteInputs:
                case ModBusFunctionCode.ReadHoldingRegisters:
                case ModBusFunctionCode.ReadInputRegisters:
                    length = buffer.GetByte(8);
                    if (buffer.Length != length + 9)
                        throw new ArgumentException(Cpr.ex_no_payload);
                    Data = new DataBuffer(buffer.Buffer.Skip(9).ToArray());
                    break;

                case ModBusFunctionCode.WriteMultipleCoils:
                case ModBusFunctionCode.WriteMultipleRegisters:
                    Address = buffer.GetUInt16(8);
                    Count = buffer.GetUInt16(10);
                    break;

                case ModBusFunctionCode.WriteSingleCoil:
                case ModBusFunctionCode.WriteSingleRegister:
                    Address = buffer.GetUInt16(8);
                    Data = new DataBuffer(buffer.Buffer.Skip(10).ToArray());
                    break;

                case ModBusFunctionCode.EncapsulatedInterface:
                    Mei = (ModBusMei)buffer.GetByte(8);
                    switch (Mei)
                    {
                        case ModBusMei.CanOpenGeneralReference:
                            Data = new DataBuffer(buffer.Buffer.Skip(9).ToArray());
                            break;

                        case ModBusMei.ReadDeviceInformation:
                            MeiCategory = (ModBusDevIdCategory)buffer.GetByte(9);
                            ConformityLevel = buffer.GetByte(10);
                            MoreRequestsNeeded = buffer.GetByte(11) > 0;
                            NextObjectId = buffer.GetByte(12);
                            ObjectCount = buffer.GetByte(13);
                            Data = new DataBuffer(buffer.Buffer.Skip(14).ToArray());
                            break;

                        default:
                            throw new NotImplementedException(Cpr.ex_unk_mei_with + Mei);
                    }
                    break;

                default:
                    throw new NotImplementedException(Cpr.ex_unk_func_with + Function);
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(' ');
        foreach (var b in Data.Buffer)
        {
	        if (sb.Length > 0)
		        sb.Append(' ');
	        sb.Append(b.ToString("X2"));
        }

        return $"#{TransactionId} (##{DeviceId}, @{Function}, E:{IsError}, {Address}[{Count}]){sb}";
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => base.GetHashCode() ^
            TransactionId.GetHashCode() ^
            DeviceId.GetHashCode() ^
            Function.GetHashCode() ^
            Address.GetHashCode() ^
            Count.GetHashCode() ^
            Data.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not Response res)
            return false;
        return res.TransactionId == TransactionId &&
            res.DeviceId == DeviceId &&
            res.Function == Function &&
            res.Address == Address &&
            res.Count == Count &&
            Data.Equals(res.Data);
    }
}
