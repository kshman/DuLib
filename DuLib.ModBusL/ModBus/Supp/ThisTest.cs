namespace Du.ModBus.Supp;

/// <summary>
/// Test 도움꾼
/// </summary>
internal static class ThisTest
{
    internal static byte[] TestLittleEndian(this byte[] bs)
    {
        return BitConverter.IsLittleEndian ? bs.Reverse().ToArray() : bs;
    }

    internal static byte[] TestReverseEndian(this byte[] bs, bool reverse)
    {
	    return reverse ? bs.Reverse().ToArray() : bs;
    }
}
