using Microsoft.AspNetCore.Components.Rendering;

namespace Du.Blazor.Components;

public class DropItem : ComponentContainer
{
	[CascadingParameter] public DropDownBase? Group { get; set; }

	[Parameter] public string? ContainerClass { get; set; }
	[Parameter] public string? Title { get; set; }

	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

	//
	protected override void OnParametersSet()
	{
		if (Group == null)
			ThrowSupp.InsideComponent(nameof(DropItem), nameof(DropDown));
	}

	//
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "li");

		builder.AddAttribute(1, "class", ContainerClass);
		builder.AddMultipleAttributes(99, UserAttrs);

		if (ChildContent is not null)
			builder.AddContent(5, ChildContent);
		else
			builder.AddContent(5, Title);

		builder.CloseElement();
	}

	//
	protected async Task HandleOnClickAsync(MouseEventArgs e)
	{
		await InvokeOnClickAsync(e);
		await Group!.InternalSetSelectedAsync(this);
	}

	//
	protected virtual Task InvokeOnClickAsync(MouseEventArgs e) => OnClick.InvokeAsync(e);
}
