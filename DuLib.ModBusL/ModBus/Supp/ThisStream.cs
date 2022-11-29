namespace Du.ModBus.Supp;

/// <summary>
/// 스트림 도움꾼
/// </summary>
internal static class ThisStream
{
	internal static async Task<byte[]> ReadExpectedBytes(this Stream st, int expected, CancellationToken cancel = default)
	{
		var bs = new byte[expected];
		var offset = 0;
		do
		{
			var count = await st.ReadAsync(bs.AsMemory(offset, expected - offset), cancel);
			if (count < 1)
			{
				var len = bs.Length - offset;
				throw new EndOfStreamException(len + Cpr.ex_no_more_by_eof_stream);
			}
			offset += count;
		} while (expected - offset > 0 && cancel.IsCancellationRequested);

		cancel.ThrowIfCancellationRequested();
		return bs;
	}
}

