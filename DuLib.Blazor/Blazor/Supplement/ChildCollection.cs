namespace Du.Blazor.Supplement;

internal class ChildCollection<T>
{
	private readonly ICollection<T> _collection;
	private readonly Func<Task> _stateHasChanged;
	private readonly Func<bool> _ownerDisposed;
	private readonly Action<T>? _itemAdded;
	private readonly Action<T>? _itemRemoved;

	public ChildCollection(ICollection<T> collection, Func<Task> stateHasChanged, Func<bool> ownerDisposed, Action<T>? itemAdded = null, Action<T>? itemRemoved = null)
	{
		_collection = collection;
		_stateHasChanged = stateHasChanged;
		_ownerDisposed = ownerDisposed;
		_itemAdded = itemAdded;
		_itemRemoved = itemRemoved;
	}

	public void Add(T item)
	{
		_collection.Add(item);
		_itemAdded?.Invoke(item);
		_stateHasChanged.Invoke();
	}

	public async void Remove(T item)
	{
		_collection.Remove(item);
		_itemRemoved?.Invoke(item);

		if (_ownerDisposed()) 
			return;

		try
		{
			await _stateHasChanged.Invoke();
		}
		catch (ObjectDisposedException)
		{
			// 이건.. 어쩔수 엄슴
		}
	}
}
