using Microsoft.AspNetCore.Components.Rendering;

namespace Du.Blazor.Components;

public class Icon : ComponentObject
{
	[Parameter] public string? Name { get; set; }

	//
	protected override void OnComponentClass(CssCompose css)
	{
		if (Name is not null)
		{
			if (Name.StartsWith("oi"))
				css.Add("oi");
			else if (Name.StartsWith("bi"))
				css.Add("bi");
	
			css.Add(Name);
		}
		else
		{
			// OI 경고
			css.Add("oi oi-warning");
		}
	}

	//
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "i");
		builder.AddAttribute(1, "class", CssClass);
		builder.AddAttribute(2, "aria-hidden", true);
		builder.AddMultipleAttributes(99, UserAttrs);
		builder.CloseElement();
	}
}
