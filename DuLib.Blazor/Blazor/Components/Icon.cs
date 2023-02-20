using Microsoft.AspNetCore.Components.Rendering;

namespace Du.Blazor.Components;

public class Icon : ComponentObject
{
	[Parameter] public string Name { get; set; } = string.Empty;

	//
	protected override void OnComponentClass(CssCompose css)
	{
		if (Name.Length > 3)
		{
			if (Name[2] == '-')
			{
				// oi / bi / fa 같은거는 세번째 빼기가 있다
				css.Add(Name[..2]);
				css.Add(Name);
			}
			else
			{
				// 없으면 걍 넣음
				css.Add(Name);
			}
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
		builder.AddAttribute(2, "aria-hidden", "true");
		builder.AddMultipleAttributes(99, UserAttrs);
		builder.CloseElement();
	}
}
