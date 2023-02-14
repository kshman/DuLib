namespace Du.Blazor.Components;

public class DuGroupBase : DuComponentParent
{
	private GroupLayout _layout;

	/// <summary>그룹 모양. <see cref="GroupLayout" /> 참고</summary>
	[Parameter]
	public GroupLayout Layout
	{
		get => _layout;
		set
		{
			if (_layout == value) return;
			_layout = value;
			CssClass.Invalidate();
		}
	}

	protected override string? RootClass =>
		_layout switch
		{
			GroupLayout.Button or
				GroupLayout.HorizontalButton => RootClasses.btn_group,
			GroupLayout.VerticalButton => RootClasses.btn_group_vertical,
			GroupLayout.ToolbarButton => RootClasses.btn_group_toolbar,
			GroupLayout.Accordion => RootClasses.accordion,
			GroupLayout.Carousel => RootClasses.carousel,
			GroupLayout.Tab => RootClasses.tab,
			_ => null
		};

	protected string? Role =>
		_layout switch
		{
			GroupLayout.Button or
				GroupLayout.HorizontalButton or
				GroupLayout.VerticalButton => RootClasses.group,
			GroupLayout.ToolbarButton => RootClasses.toolbar,
			_ => null
		};

	protected string? GetButtonLayout(ComponentSize size = ComponentSize.Medium)
	{
		return _layout switch
		{
			GroupLayout.Button or
				GroupLayout.HorizontalButton or
				GroupLayout.VerticalButton or
				GroupLayout.ToolbarButton => size.ToCss(RootClasses.btn_group),
			_ => null
		};
	}
}
