namespace Du.ModBus.Supp;

/// <summary>
/// ReaderWriterLockSlim 도움꾼 
/// </summary>
internal static class ThisRwLockSlim
{
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
}
