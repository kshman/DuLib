namespace Du.Blazor.Components;

/// <summary>
/// 자식 개체 관리하는 그룹
/// </summary>
/// <remarks>
/// 엌... 이거 상속 안되네... 왜 만들엇음<br/>
/// 얘를 제네릭없이 상속 시키면 됨 <see cref="DuGroupParentCarousel"/> <see cref="DuGroupParentTab"/>
/// </remarks>
public abstract class DuGroupParent<T> : DuComponentParent, IAsyncDisposable where T : DuComponentParent
{
	[Parameter] public string? Active { get; set; }
	[Parameter] public EventCallback<string?> ActiveChanged { get; set; }

	protected readonly List<T> _items = new();
	protected T? _current;

	//
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (!firstRender)
			return;

		// AddItemAsync 에서 처리했을 텐데 혹시나
		if (_current is null)
		{
			if (Active.IsHave(true))
				await SelectItemAsync(Active);
			else if (_items.Count > 0)
				await SelectItemAsync(_items[0]);
		}

		//
		var task = OnAfterFirstRenderAsync();
		if (task.ShouldAwaitTask())
		{
			try
			{
				await task;
			}
			catch
			{
				if (!task.IsCanceled) throw;
			}
		}
	}

	//
	public async Task AddItemAsync(T item)
	{
		_items.Add(item);

		if (Active is not null)
		{
			if (item.Id == Active)
				await SelectItemAsync(item);
		}
		else if (item.Disabled is false && _current is null)
		{
			await SelectItemAsync(item);
		}

		var task = OnItemAddedAsync(item);
		await StateHasChangedOnAsyncCompletion(task);
	}

	//
	public async Task RemoveItemAsync(T item)
	{
		if (_current == item)
			_current = null;

		_items.Remove(item);

		try
		{
			var task = OnItemRemovedAsync(item);
			await StateHasChangedOnAsyncCompletion(task);
		}
		catch (ObjectDisposedException)
		{
			// 하다가 없어지는 경우가 있긴함
		}
	}

	//
	protected async Task SelectItemAsync(T? item)
	{
		if (item == _current)
			return;

		var previous = _current;
		_current = item;

		var task = OnItemSelectedAsync(item, previous);
		if (task.ShouldAwaitTask())
		{
			try
			{
				await task;
			}
			catch
			{
				if (task.IsCanceled) return;
				throw;
			}
		}

		if (item is not null && task.Result)
			await InvokeActiveChangedAsync(item.Id);
	}

	//
	private Task SelectItemAsync(string? id)
	{
		var item = _items.FirstOrDefault(i => i.Id == id);
		return SelectItemAsync(item);
	}

	//
	protected virtual Task OnAfterFirstRenderAsync() =>
		Task.CompletedTask;

	//
	protected virtual Task OnItemAddedAsync(T item) =>
		Task.CompletedTask;

	//
	protected virtual Task OnItemRemovedAsync(T item) =>
		Task.CompletedTask;

	// false를 반환하면 그뒤에 기능(InvokeActiveChangedAsync 호출)을 하지 않음
	protected virtual Task<bool> OnItemSelectedAsync(T? item, T? previous) =>
		Task.FromResult(true);

	//
	protected virtual Task InvokeActiveChangedAsync(string? id) =>
		ActiveChanged.InvokeAsync(id);

	//
	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	//
	protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;
}

//
public abstract class DuGroupParentCarousel : DuGroupParent<DuCarousel>
{
}

//
public abstract class DuGroupParentTab : DuGroupParent<DuTab>
{
}

//
public abstract class DuGrouParentPivot : DuGroupParent<DuPivot>
{
}
