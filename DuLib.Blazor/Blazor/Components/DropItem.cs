namespace Du.Blazor.Components;

public abstract class DropItem : ComponentContainer
{
	[CascadingParameter] public DropDown? Group { get; set; }

	[Parameter] public string? ListClass { get; set; }
	[Parameter] public string? Title { get; set; }

	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

	//
	protected override void OnParametersSet()
	{
		if (Group == null)
			ThrowSupp.InsideComponent(nameof(DropItem), nameof(DropDown));
	}

	//
	protected async Task HandleOnClickAsync(MouseEventArgs e)
	{
		await InvokeOnClickAsync(e);
		await Group!.SetSelectedAsync(this);
	}

	//
	protected virtual Task InvokeOnClickAsync(MouseEventArgs e) => OnClick.InvokeAsync(e);
}
