using Du.Blazor.Components;

namespace Du.Blazor;

/// <summary>
/// 컴포넌트 이벤트
/// </summary>
public class DuComponentEvent : EventArgs
{
}

/// <summary>
/// 아코디언 펼침 이벤트
/// </summary>
public class AccordionExpandEvent : DuComponentEvent
{
	public string Id { get; set; }
	public bool Expanded { get; set; }

	public AccordionExpandEvent(string id, bool expanded)
	{
		Id = id;
		Expanded = expanded;
	}
}

/// <summary>
/// Carousel 이벤트
/// </summary>
public class CarouselSlideEvent : DuComponentEvent
{
	public int From { get; set; }
	public int To { get; set; }
	public bool IsLeft { get; set; }
}

/// <summary>
/// 피벗 이벤트
/// </summary>
public class PivotChangeEvent : DuComponentEvent
{
	public DuPivot? Current { get; set; }
	public DuPivot? Previous { get; set; }
}

/// <summary>
/// 탭 이벤트
/// </summary>
public class TabChangeEvent : DuComponentEvent
{
	public DuTab? Current { get; set; }
	public DuTab? Previous { get; set; }
}
