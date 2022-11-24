namespace Du.ModBus.Supp;

/// <summary>
/// 타입 서포트
/// </summary>
internal static class ThisType
{
    internal static T? GetAttribute<T>(this Enum e) where T : Attribute
    {
        var f = e.GetType().GetField(e.ToString());
        var a = f?.GetCustomAttributes(typeof(T), inherit: false) as T[];
        return a?.FirstOrDefault();
    }

    internal static string GetDescription(this Enum e)
    {
        return e.GetAttribute<DescriptionAttribute>()?.Description ?? e.ToString();
    }

    internal static string GetMessage(this Exception ex)
        => ex.InnerException?.Message ?? ex.Message;

    internal static async void Forget(this Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // ignored
        }
    }

    internal static IDisposable GetReadLock(this ReaderWriterLockSlim l, int millisecondsTimeout = -1)
    {
        if (!l.TryEnterReadLock(millisecondsTimeout))
            throw new TimeoutException(Cpr.ex_enter_read);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.Read);
    }

    internal static IDisposable GetReadLock(this ReaderWriterLockSlim l, TimeSpan timeSpan)
    {
        if (!l.TryEnterReadLock(timeSpan))
            throw new TimeoutException(Cpr.ex_enter_read);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.Read);
    }

    internal static IDisposable GetUpgradableReadLock(this ReaderWriterLockSlim l, int millisecondsTimeout = -1)
    {
        if (!l.TryEnterUpgradeableReadLock(millisecondsTimeout))
            throw new TimeoutException(Cpr.ex_enter_upgradable_read);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.UpgradableRead);
    }

    internal static IDisposable GetUpgradableReadLock(this ReaderWriterLockSlim l, TimeSpan timeSpan)
    {
        if (!l.TryEnterUpgradeableReadLock(timeSpan))
            throw new TimeoutException(Cpr.ex_enter_upgradable_read);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.UpgradableRead);
    }

    internal static IDisposable GetWriteLock(this ReaderWriterLockSlim l, int millisecondsTimeout = -1)
    {
        if (!l.TryEnterWriteLock(millisecondsTimeout))
            throw new TimeoutException(Cpr.ex_enter_write);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.Write);
    }

    internal static IDisposable GetWriteLock(this ReaderWriterLockSlim l, TimeSpan timeSpan)
    {
        if (!l.TryEnterWriteLock(timeSpan))
            throw new TimeoutException(Cpr.ex_enter_write);
        return new DisposableReaderWriterLockSlim(l, DisposableReaderWriterLockSlim.Mode.Write);
    }

    private class DisposableReaderWriterLockSlim : IDisposable
    {
        private readonly ReaderWriterLockSlim _l;
        private Mode _m;

        public DisposableReaderWriterLockSlim(ReaderWriterLockSlim l, Mode m)
        {
            _l = l;
            _m = m;
        }

        public void Dispose()
        {
            switch (_m)
            {
                case Mode.None:
                    return;

                case Mode.Read:
                    _l.ExitReadLock();
                    break;

                case Mode.UpgradableRead when _l.IsWriteLockHeld:
                    _l.ExitWriteLock();
                    break;

                case Mode.UpgradableRead:
                    _l.ExitUpgradeableReadLock();
                    break;

                case Mode.Write:
                    _l.ExitWriteLock();
                    break;

                default:
                    _m = Mode.None;
                    break;
            }
        }

        public enum Mode
        {
            None,
            Read,
            UpgradableRead,
            Write,
        }
    }

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

    internal static byte[] TestLittleEndian(this byte[] bs)
    {
        return BitConverter.IsLittleEndian ? bs.Reverse().ToArray() : bs;
    }

    internal static byte[] TestReverseEndian(this byte[] bs, bool reverse)
    {
	    return reverse ? bs.Reverse().ToArray() : bs;
    }
}
