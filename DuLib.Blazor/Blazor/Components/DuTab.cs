namespace Du.Blazor.Components;

/// <summary>탭 아이템</summary>
public class DuTab : DuComponentParent, IDisposable
{
	/// <summary>그룹</summary>
	[CascadingParameter] public DuGroupTab? Group { get; set; }

	/// <summary>타이틀 <see cref="Header"/></summary>
	[Parameter] public string? Title { get; set; }
	/// <summary>순번</summary>
	[Parameter] public int Index { get; set; }

	/// <summary>헤더 <see cref="Title"/></summary>
	[Parameter] public RenderFragment? Header { get; set; }
	/// <summary>내용 
	/// <see cref="Header"/>와 짝꿍<br/>이 내용이 있을 경우,
	/// 태그 밖 <see cref="DuComponentParent.ChildContent"/>는 처리하지 않는다
	/// </summary>
	[Parameter] public RenderFragment? Content { get; set; }

	protected override string RootClass => RootClasses.tab_item;
	protected override string RootId => RootIds.tab;

	private bool _disposed;

	//
	protected override void OnComponentInitialized()
	{
		Group?.AddTab(this);
	}

	//
	public void Dispose()
	{
		if (_disposed)
			return;

		Group?.RemoveTab(this);

		_disposed = true;
	}
}
