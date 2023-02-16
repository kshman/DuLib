namespace Du.Blazor.Components;

public class DuButtonBase : DuComponentParent
{
	/// <summary>에디트 컨텍스트</summary>
	[CascadingParameter] public EditContext? EditContext { get; set; }

	/// <summary>버튼 타입. <see cref="ButtonType" /> 참고</summary>
	[Parameter] public ButtonType? Type { get; set; }
	/// <summary>레이아웃 타입. <see cref="ComponentColor" /> 참고</summary>
	[Parameter]public ComponentColor Color { get; set; }
	/// <summary>컴포넌트 크기. <see cref="ComponentSize" /> 참고</summary>
	[Parameter]public ComponentSize Size { get; set; }
	/// <summary>아웃라인 적용.</summary>
	[Parameter]public bool Outline { get; set; }

	/// <summary>마우스 눌린 이벤트 지정.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
	/// <summary>에디트 폼 ValidClick.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnValidClick { get; set; }
	/// <summary>에디트 폼 InvalidClick.</summary>
	[Parameter] public EventCallback<MouseEventArgs> OnInvalidClick { get; set; }

	protected override string RootClass => "btn";

	private bool _handle_click;

	// OnComponentInitialized를 안쓰고 이걸 쓴 이유는... 컴포넌트 베이스니깐
	protected override void OnInitialized()
	{
		Type ??= EditContext is null ? ButtonType.Button : ButtonType.Submit;

		base.OnInitialized();
	}

	//
	protected virtual Task InvokeOnClickAsync(MouseEventArgs e) =>
		OnClick.InvokeAsync(e);

	//
	protected virtual Task InvokeOnValidClickAsync(MouseEventArgs e) =>
		OnValidClick.InvokeAsync(e);

	//
	protected virtual Task InvokeOnInvalidClickAsync(MouseEventArgs e) =>
		OnInvalidClick.InvokeAsync(e);

	// 마우스 핸들러
	protected async Task HandleOnClickAsync(MouseEventArgs e)
	{
		if (!_handle_click)
		{
			_handle_click = true;

			if (OnClick.HasDelegate)
				await InvokeOnClickAsync(e);
			else if (Type == ButtonType.Submit && EditContext != null)
				switch (EditContext.Validate())
				{
					case true when OnValidClick.HasDelegate:
						await InvokeOnValidClickAsync(e);
						break;

					case false when OnInvalidClick.HasDelegate:
						await InvokeOnInvalidClickAsync(e);
						break;
				}

			_handle_click = false;
		}
	}

	//
	protected string? GetHtmlType() => Type switch
	{
		ButtonType.Button => "button",
		ButtonType.Submit => "submit",
		ButtonType.Reset => "reset",
		_ => null
	};

	//
	protected string GetCssColor() => Color == ComponentColor.Link
			? "btn-link"
			: Color.ToCss(Outline ? "btn-outline" : "btn");

	//
	protected string? GetCssSize() =>
		Size.ToCss("btn");
}
