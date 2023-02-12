using DuLib.Blazor.Supplement;
using Microsoft.AspNetCore.Components;

namespace DuLib.Blazor;

/// <summary>
/// 컴포넌트 기본
/// </summary>
public abstract class DuComponentBase : ComponentBase
{
	/// <summary>테마 지정</summary>
	[CascadingParameter] public ThemeStyle Theme { get; set; }
	/// <summary>스타일 지정</summary>
	[Parameter] public string? Style { get; set; }
	/// <summary>클래스 지정</summary>
	[Parameter] public string? Class { get; set; }
	/// <summary>사용 여부 지정</summary>
	[Parameter] public bool Enabled { get; set; } = true;
	/// <summary>표현 방식 지정</summary>
	[Parameter] public ComponentVisibility Visibility { get; set; }
	/// <summary>사용자가 설정한 속성 지정</summary>
	[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> UserAttributes { get; set; } 
		= new();

	/// <summary>루트 엘리먼트</summary>
	protected ElementReference RootElement { get; set; }
	/// <summary>클래스 이름<br/>이 값이 기본 클래스가 된다</summary>
	protected virtual string? RootClass { get; }
	/// <summary>스타일 만들기 도움꾼</summary>
	
	protected StyleCompose CssStyle { get; init; } = new StyleCompose();
	/// <summary>클래스 만들기 도움꾼</summary>
	protected ClassCompose CssClass { get; init; } = new ClassCompose();
	/// <summary><see cref="OnAfterRender"/> 메소드가 처리되면 참</summary>
	protected bool Rendered { get; private set; }

	/// <summary>
	/// 초기화 + 컴포넌트 속성 만들기.<br/>
	/// 내부에서 컴포넌트 속성을 만들기에 만약 이 메소드를 재정의 한다면,
	/// 재정의한 곳에서 <i>base.OnInitialized()</i> 메소드를 호출할 것
	/// <code>
	/// protected override void OnInitialized()
	/// {
	///		// 여기에 파라미터 처리
	///		base.OnInitialized();
	///		// 여기에 스타일/클래스 코드
	///		// 여기에 기타 사용자 코드
	/// }
	/// </code>
	/// </summary>
	protected override void OnInitialized()
	{
		OnCssStyle();
		CssStyle
			.Add(Style)
			.Add(Visibility.ToCss());

		CssClass
			.Add(RootClass)
			.Add(!Enabled, "disabled");

		OnCssClass();
		CssClass
			.Add(Class);
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

	/// <summary>컴포넌트의 스타일 값을 지정</summary>
	/// <seealso cref="OnCssClass"/>
	protected virtual void OnCssStyle()
	{ }

	/// <summary>컴포넌트의 클래스 값을 지정</summary>
	/// <seealso cref="OnCssStyle"/>
	protected virtual void OnCssClass()
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

	//
	protected override void OnInitialized()
	{
		Id ??= $"D_CP_{TypeSupp.Increment}";

		base.OnInitialized();
	}
}
