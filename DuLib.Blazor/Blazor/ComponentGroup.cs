namespace Du.Blazor;

/// <summary>
/// 자식 개체 관리하는 그룹 컴포넌트
/// </summary>
/// <remarks>
/// 엌... 이거 상속 안되네... 왜 만들엇음<br/>
/// 얘를 제네릭없이 상속 시키면 됨<br/>
/// </remarks>
public abstract class ComponentGroup<T> : ComponentContainer, IAsyncDisposable 
	where T : ComponentContainer
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

	/// <summary>
	/// 아이템 추가
	/// </summary>
	/// <param name="item"></param>
	/// <returns>비동기 처리한 태스크</returns>
	public async Task AddItemAsync(T item)
	{
		_items.Add(item);

		if (Active is not null)
		{
			if (item.Id == Active)
				await SelectItemAsync(item);
		}
		else if (item.Enabled && _current is null)
		{
			await SelectItemAsync(item);
		}

		var task = OnItemAddedAsync(item);
		await StateHasChangedOnAsyncCompletion(task);
	}

	/// <summary>
	/// 아이템 삭제
	/// </summary>
	/// <param name="item"></param>
	/// <returns>비동기 처리한 태스크</returns>
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

	/// <summary>
	/// 아이템 선택
	/// </summary>
	/// <param name="item"></param>
	/// <returns>비동기 처리한 태스크</returns>
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

	/// <summary>
	/// 아이디로 아이템 선택
	/// </summary>
	/// <param name="id"></param>
	/// <returns>비동기 처리한 태스크</returns>
	private Task SelectItemAsync(string? id)
	{
		var item = _items.FirstOrDefault(i => i.Id == id);
		return SelectItemAsync(item);
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	/// <summary><see cref="DisposeAsync"/>의 처리 메소드</summary>
	/// <returns>비동기 처리한 태스크</returns>
	protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

	/// <summary>
	/// <see cref="OnAfterRenderAsync(bool)"/> 메소드가 처리될 때
	/// 특별하게 firstRender의 경우에만 호출
	/// </summary>
	/// <returns>비동기 처리한 태스크</returns>
	protected virtual Task OnAfterFirstRenderAsync() => Task.CompletedTask;

	/// <summary>
	/// 아이템 추가될 때
	/// </summary>
	/// <param name="item"></param>
	/// <returns>비동기 처리한 태스크</returns>
	protected virtual Task OnItemAddedAsync(T item) => Task.CompletedTask;

	/// <summary>
	/// 아이템 삭제할 때 
	/// </summary>
	/// <param name="item"></param>
	/// <returns>비동기 처리한 태스크</returns>
	protected virtual Task OnItemRemovedAsync(T item) => Task.CompletedTask;

	/// <summary>
	/// 아이템을 선택했을 때
	/// </summary>
	/// <param name="item"></param>
	/// <param name="previous"></param>
	/// <returns>
	/// 비동기 처리한 태스크<br/>
	/// false를 반환하면 그뒤에 기능(InvokeActiveChangedAsync 호출)을 하지 않음
	/// </returns>
	protected virtual Task<bool> OnItemSelectedAsync(T? item, T? previous) => Task.FromResult(true);

	/// <summary>
	/// <see cref="Active"/> 항목이 변경됐을 때
	/// </summary>
	/// <param name="id"></param>
	/// <returns>비동기 처리한 태스크</returns>
	protected virtual Task InvokeActiveChangedAsync(string? id) => ActiveChanged.InvokeAsync(id);
}
