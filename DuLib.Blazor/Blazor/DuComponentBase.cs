namespace Du.Blazor;

/// <summary>
/// 컴포넌트 기본
/// </summary>
public abstract class DuComponentBase : ComponentBase
{
	/// <summary>테마 지정 (data-bs-theme)</summary>
	[CascadingParameter] public ThemeStyle Theme { get; set; }
	/// <summary>사용하지 않음 지정 (disabled)</summary>
	[Parameter] public bool Disabled { get; set; }
	/// <summary>클래스 지정</summary>
	[Parameter] public string? Class { get; set; }
	/// <summary>사용자가 설정한 속성 지정</summary>
	[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> UserAttrs { get; set; } = new();

	/// <summary>
	/// 클래스 이름<br/>
	/// 이 값이 기본 클래스가 된다
	/// </summary>
	protected virtual string RootName => RootNames.empty;
	/// <summary>루트 엘리먼트</summary>
	protected ElementReference RootElement { get; set; }
	/// <summary>CSS 클래스 </summary>
	protected string RootClass => _css.Class;

	// CSS 컴포즈
	private readonly CssSupp _css = new();

	/// <summary>
	/// 초기화 + 컴포넌트 속성 만들기.<br/>
	/// 내부에서 컴포넌트 속성을 만들기에, 초기화 할 때는 이 메소드를 재정의 하지 말고,
	/// <see cref="OnComponentInitialized"/> 메소드를 재정의해서 쓸 것.
	/// </summary>
	protected override void OnInitialized()
	{
		_css.Set(RootName).Register(CheckCssDisabled);
		OnComponentClass(_css);
		_css.Add(Class);

		OnComponentInitialized();
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

	/// <summary>컴포넌트의 CSS 클래스 값을 지정</summary>
	protected virtual void OnComponentClass(CssSupp css)
	{ }

	protected async Task StateHasChangedAsync() =>
		await InvokeAsync(StateHasChanged);

	private string? CheckCssDisabled() =>
		Disabled.IfTrue(CssConsts.disabled);
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
