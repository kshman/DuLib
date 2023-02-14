namespace Du.Blazor.Components;

public class DuTab : DuComponentParent, IDisposable
{
	[CascadingParameter]
	public DuGroupTab? Group { get; set; }

	[Parameter]
	public string? AriaLabel { get; set; }

	[Parameter]
	public string? Title { get; set; }

	[Parameter]
	public RenderFragment? Header { get; set; }

	[Parameter]
	public RenderFragment? Content { get; set; }

	public bool Selected
	{
		get => _selected;
		set
		{
			if (value == _selected) return;
			_selected = value;
			CssClass.Invalidate();
		}
	}

	protected override string RootClass => ""; //RootClasses.tab_item;

	private bool _selected;
	private bool _disposed;

	//
	protected override void OnComponentInitialized()
	{
		Group?.AddTab(this);
	}

	//
	protected override void OnComponentClass()
	{
		//CssClass
		//	.Add(() => Selected.IfTrue(RootClasses.tab_sel));
	}

	//
	public void SetSelected(bool selected)
	{
		Selected = selected;
		StateHasChanged();
	}

	//
	private void HandleOnClick()
	{
		if (Enabled)
			Group?.SelectTab(this);
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
