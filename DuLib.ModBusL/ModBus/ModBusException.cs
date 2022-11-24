namespace Du.ModBus;

/// <summary>
/// 모드버스 예외
/// </summary>
public class ModBusException : Exception
{
	public ModBusException() { }

	public ModBusException(string message) : base(message) { }

	public ModBusException(string message, Exception innerException) : base(message, innerException) { }

	public ModBusErrorCode ErrorCode { get; set; }

	public string ErrorMessage => ErrorCode.GetDescription();
}
