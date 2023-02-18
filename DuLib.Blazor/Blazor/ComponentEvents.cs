using Du.Blazor.Components;

namespace Du.Blazor;

/// <summary>
/// 아코디언 펼침 이벤트
/// </summary>
public class AccordionExpandEvent : EventArgs
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
public class CarouselSlideEvent : EventArgs
{
	public int From { get; set; }
	public int To { get; set; }
	public bool IsLeft { get; set; }
}
