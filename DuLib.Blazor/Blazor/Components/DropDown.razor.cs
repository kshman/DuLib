namespace Du.Blazor.Components;

/// <summary>
/// 드롭 버튼
/// </summary>
public partial class DropDown
{
}

/// <summary>
/// 드롭 버튼 기본
/// </summary>
public abstract class DropDownBase : ComponentContainer
{
	[Parameter] public bool AutoClose { get; set; } = true;
	[Parameter] public DropDirection Direction { get; set; }
	[Parameter] public DropAlignment Alignment { get; set; }

	[Parameter] public string? ContainerClass { get; set; }
	[Parameter] public string? MenuClass { get; set; }

	[Parameter] public string? Title { get; set; }
	[Parameter] public RenderFragment? Display { get; set; }
	[Parameter] public RenderFragment? List { get; set; }

	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
	[Parameter] public EventCallback<bool> OnShowHide { get; set; }
	[Parameter] public EventCallback<SelectEventArgs> OnSelect { get; set; }

	public bool IsOpen { get; set; }
	public DropItem? SelectedItem { get; set; }

	//
	protected override string CssName => "dropdown-toggle";

	//
	private bool _handle_click;

	//
	private async Task SetOpen(bool isOpen)
	{
		IsOpen = isOpen;
		await InvokeOnShowHideAsync(isOpen);
		StateHasChanged();
	}

	//
	public Task ShowAsync() =>
		SetOpen(true);

	//
	public Task HideAsync() =>
		SetOpen(false);

	//
	public Task ToggleAsync() =>
		SetOpen(!IsOpen);

	//
	protected async Task SetSelectedAsync(DropItem item)
	{
		SelectedItem = item;
		await SetOpen(false);
		await InvokeOnSelectAsync(new SelectEventArgs(item.Id, item.Title));
	}

	//
	internal Task InternalSetSelectedAsync(DropItem item) => SetSelectedAsync(item);

	//
	protected async Task HandleFocusOutAsync()
	{
		if (AutoClose is false)
			return;
		if (IsOpen is false)
			return;
		await HideAsync();
	}

	//
	protected async Task HandleOnClickAsync(MouseEventArgs e)
	{
		if (_handle_click is false)
		{
			_handle_click = true;

			if (OnClick.HasDelegate)
				await InvokeOnClickAsync(e);

			await ToggleAsync();

			_handle_click = false;
		}
	}

	//
	protected virtual Task InvokeOnClickAsync(MouseEventArgs e) => OnClick.InvokeAsync(e);
	protected virtual Task InvokeOnShowHideAsync(bool e) => OnShowHide.InvokeAsync(e);
	protected virtual Task InvokeOnSelectAsync(SelectEventArgs e) => OnSelect.InvokeAsync(e);

	//
	protected string MenuCssClass => CssCompose.Join(
		"dropdown-menu",
		Alignment.ToCss(),
		MenuClass,
		IsOpen ? "show" : null)!;
}
