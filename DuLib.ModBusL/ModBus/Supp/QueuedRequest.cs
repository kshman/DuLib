namespace Du.ModBus.Supp;

/// <summary>
/// 리퀘스트 큐
/// </summary>
internal class QueuedRequest
{
    public ushort TransactionId { get; set; }
    public CancellationTokenRegistration Registration { get; set; }
    public TaskCompletionSource<Response>? TaskCompletionSource { get; set; }
    public CancellationTokenSource? CancellationTokenSource { get; set; }
    public CancellationTokenSource? TimeoutCancellationTokenSource { get; set; }
}
