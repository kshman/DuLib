namespace Du.Blazor;

/// <summary>
/// 컴포넌트 기본
/// </summary>
public abstract class DuComponentBase : ComponentBase
{
	/// <summary>테마 지정</summary>
	[CascadingParameter]
	public ThemeStyle Theme
	{
		get => _theme;
		set
		{
			if (_theme == value) return;
			_theme = value;
			CssClass.Invalidate();
		}
	}

	/// <summary>사용 여부 지정</summary>
	[Parameter]
	public bool Enabled
	{
		get => _enabled;
		set
		{
			if (_enabled == value) return;
			_enabled = value;
			CssClass.Invalidate();
		}
	}
	/// <summary>스타일 지정</summary>
	[Parameter]
	public string? Style
	{
		get => _style;
		set
		{
			if (_style == value) return;
			_style = value;
			CssClass.Invalidate();
		}
	}
	/// <summary>클래스 지정</summary>
	[Parameter]
	public string? Class
	{
		get => _class;
		set
		{
			if (_class == value) return;
			_class = value;
			CssClass.Invalidate();
		}
	}
	/// <summary>표현 방식 지정</summary>
	[Parameter]
	public ComponentVisibility Visibility
	{
		get => _visibility;
		set
		{
			if (_visibility == value) return;
			_visibility = value;
			CssClass.Invalidate();
		}
	}
	/// <summary>사용자가 설정한 속성 지정</summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object> UserAttributes { get; set; } = new();

	/// <summary>루트 엘리먼트</summary>
	protected ElementReference RootElement { get; set; }
	/// <summary>클래스 이름<br/>이 값이 기본 클래스가 된다</summary>
	protected abstract string? RootClass { get; }
	/// <summary>스타일 만들기 도움꾼</summary>

	protected StyleCompose CssStyle { get; init; } = new();
	/// <summary>클래스 만들기 도움꾼</summary>
	protected ClassCompose CssClass { get; init; } = new();
	/// <summary><see cref="OnAfterRender"/> 메소드가 처리되면 참</summary>
	protected bool Rendered { get; private set; }

	// 내부 변수
	private ThemeStyle _theme;

	private bool _enabled = true;
	private string? _style;
	private string? _class;
	private ComponentVisibility _visibility;

	/// <summary>
	/// 초기화 + 컴포넌트 속성 만들기.<br/>
	/// 내부에서 컴포넌트 속성을 만들기에, 초기화 할 때는 이 메소드를 재정의 하지 말고,
	/// <see cref="OnComponentInitialized"/> 메소드를 재정의해서 쓸 것.
	/// </summary>
	protected override void OnInitialized()
	{
		OnComponentStyle();
		CssStyle
			.Add(() => _style)
			.Add(() => _visibility.ToCss());

		CssClass
			.Add(() => RootClass)
			.Add(() => _enabled.IfFalse("disabled"));
		OnComponentClass();
		CssClass
			.Add(() => _class);

		OnComponentInitialized();
	}

	/// <summary>
	/// 렌더 후 처리.<br/>
	/// 내부에서 <see cref="Rendered"/> 값을 설정하므로 재정의한 곳 처음 또는 마지막에
	/// <i>base.OnAfterRender()</i>  메소드를 호출할 것
	/// <code>
	/// protected override void OnAfterRender()
	/// {
	///		base.OnAfterRender();
	///		// 여기에 사용자 코드
	/// }
	/// </code>
	/// </summary>
	protected override void OnAfterRender(bool firstRender)
	{
		Rendered = true;
	}

	/// <summary>
	/// 컴포넌트가 초기화 될 때 호출.<br/>
	/// <see cref="OnInitialized"/> 메소드를 재정의 하지 말고 이걸 재정의 할것.<br/>
	/// <br/>
	/// 한편, 특별할 일 없는한 비었기 땜에 부모 호출 안해도 됨. <br/>
	/// 또한 비동기 버전은 없기 때문에 그럴 때는 
	/// <see cref="ComponentBase.OnInitializedAsync"/> 메소드를 재정의해서 쓸 것.
	/// </summary>
	protected virtual void OnComponentInitialized()
	{ }

	/// <summary>컴포넌트의 스타일 값을 지정</summary>
	/// <seealso cref="OnComponentClass"/>
	protected virtual void OnComponentStyle()
	{ }

	/// <summary>컴포넌트의 클래스 값을 지정</summary>
	/// <seealso cref="OnComponentStyle"/>
	protected virtual void OnComponentClass()
	{ }
}

/// <summary>
/// 자식을 가지는 컴포넌트 기본
/// </summary>
public abstract class DuComponentParent : DuComponentBase
{
	/// <summary>자식 콘텐트</summary>
	[Parameter] public RenderFragment? ChildContent { get; set; }
	/// <summary>컴포넌트 아이디</summary>
	[Parameter] public string? Id { get; set; }

	/// <summary>루트 아이디 앞문장</summary>
	protected virtual string RootId => RootIds.parent;

	/// <inheritdoc/>>
	protected override void OnInitialized()
	{
		Id ??= $"{RootId}_{TypeSupp.Increment}";

		base.OnInitialized();
	}
}
