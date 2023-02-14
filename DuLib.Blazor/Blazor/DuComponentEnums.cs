namespace Du.Blazor;

/// <summary>
/// 테마 스타일
/// </summary>
public enum ThemeStyle
{
	Light,
	Dark,
}

/// <summary>
/// 컴포넌트 표시 방법
/// </summary>
public enum ComponentVisibility
{
	Visible,
	Hidden,
	Collapsed,
}

/// <summary>
/// 컴포넌트 크기
/// </summary>
public enum ComponentSize
{
	Medium,
	Small,
	Large,
}

/// <summary>
/// 컴포넌트 색깔
/// </summary>
public enum ComponentColor
{
	Primary,
	Secondary,
	Success,
	Danger,
	Warning,
	Info,
	Light,
	Dark,
	Link,
}

/// <summary>
/// 오버플로우 일때 처리
/// </summary>
public enum ComponentOverflow
{
	None,
	Menu,
	Scroll,
}

public enum ComponentPosition
{
	Top,
	Right,
	Bottom, 
	Left, 
	//Center,	
}

/// <summary>
/// 버튼 기능
/// </summary>
public enum ButtonType
{
	Button,
	Submit,
	Reset,
}

/// <summary>
/// 그룹 레이아웃
/// </summary>
public enum GroupLayout
{
	None,

	Button,
	HorizontalButton,
	VerticalButton,
	ToolbarButton,

	Accordion,
	Carousel,
	Tab,
}

/// <summary>
/// carousel 정지 방법
/// </summary>
public enum CarouselPause
{
	Hover,
	False,
}

/// <summary>
/// carousel 재생 방법
/// </summary>
public enum CarouselPlay
{
	Auto,	// 자동
	False,	// 수동
	True,	// 한 사이클
}

/// <summary>
/// 탭 타이틀 레이아웃
/// </summary>
public enum TabTitleLayout
{
	Flat,
	Box,
}
