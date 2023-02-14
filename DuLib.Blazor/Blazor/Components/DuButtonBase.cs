namespace Du.Blazor.Components;

public class DuButtonBase : DuComponentParent
{
	// 내부 변수
	private ComponentColor _component_color;
	private ComponentSize _component_size;
	private bool _handle_click;
	private bool _outline;

	/// <summary>에디트 컨텍스트</summary>
	[CascadingParameter]
	public EditContext? EditContext { get; set; }

	/// <summary>버튼 타입. <see cref="ButtonType" /> 참고</summary>
	[Parameter]
	public ButtonType? Type { get; set; }

	/// <summary>레이아웃 타입. <see cref="ComponentColor" /> 참고</summary>
	[Parameter]
	public ComponentColor Color
	{
		get => _component_color;
		set
		{
			if (_component_color == value) return;
			_component_color = value;
			CssClass.Invalidate();
		}
	}

	/// <summary>컴포넌트 크기. <see cref="ComponentSize" /> 참고</summary>
	[Parameter]
	public ComponentSize Size
	{
		get => _component_size;
		set
		{
			if (_component_size == value) return;
			_component_size = value;
			CssClass.Invalidate();
		}
	}

	/// <summary>아웃라인 적용.</summary>
	[Parameter]
	public bool Outline
	{
		get => _outline;
		set
		{
			if (_outline == value) return;
			_outline = value;
			CssClass.Invalidate();
		}
	}

	/// <summary>마우스 눌린 이벤트 지정.</summary>
	[Parameter]
	public EventCallback<MouseEventArgs> OnClick { get; set; }

	/// <summary>에디트 폼 ValidClick.</summary>
	[Parameter]
	public EventCallback<MouseEventArgs> OnValidClick { get; set; }

	/// <summary>에디트 폼 InvalidClick.</summary>
	[Parameter]
	public EventCallback<MouseEventArgs> OnInvalidClick { get; set; }

	protected override string RootClass => RootClasses.btn;
	protected override string RootId => RootIds.button;

	// OnComponentInitialized를 안쓰고 이걸 쓴 이유는... 컴포넌트 베이스니깐
	protected override void OnInitialized()
	{
		Type ??= EditContext is null ? ButtonType.Button : ButtonType.Submit;

		base.OnInitialized();
	}

	//
	protected virtual Task InvokeOnClick(MouseEventArgs e)
	{
		return OnClick.InvokeAsync(e);
	}

	//
	protected virtual Task InvokeOnValidClick(MouseEventArgs e)
	{
		return OnValidClick.InvokeAsync(e);
	}

	//
	protected virtual Task InvokeOnInvalidClick(MouseEventArgs e)
	{
		return OnInvalidClick.InvokeAsync(e);
	}

	// 마우스 핸들러
	protected async Task HandleOnClick(MouseEventArgs e)
	{
		if (!Enabled)
			return;

		if (!_handle_click)
		{
			_handle_click = true;

			if (OnClick.HasDelegate)
				await InvokeOnClick(e);
			else if (Type == ButtonType.Submit && EditContext != null &&
					 (OnValidClick.HasDelegate || OnInvalidClick.HasDelegate))
				switch (EditContext.Validate())
				{
					case true when OnValidClick.HasDelegate:
						await InvokeOnValidClick(e);
						break;

					case false when OnInvalidClick.HasDelegate:
						await InvokeOnInvalidClick(e);
						break;
				}

			_handle_click = false;
		}
	}

	//
	protected string? GetTypeHtml()
	{
		return Type switch
		{
			ButtonType.Button => "button",
			ButtonType.Submit => "submit",
			ButtonType.Reset => "reset",
			_ => null
		};
	}

	//
	protected string GetColorCss()
	{
		return Color == ComponentColor.Link
			? RootClasses.btn_link
			: Color.ToCss(Outline ? RootClasses.btn_outline : RootClasses.btn);
	}

	//
	protected string? GetSizeCss()
	{
		return Size.ToCss(RootClasses.btn);
	}
}
