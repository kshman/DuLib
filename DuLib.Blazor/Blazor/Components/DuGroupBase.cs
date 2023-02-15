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

	protected override string? RootName => _layout switch
	{
		GroupLayout.Button or
			GroupLayout.HorizontalButton => RootNames.btn_group,
		GroupLayout.VerticalButton => RootNames.btn_group_vertical,
		GroupLayout.ToolbarButton => RootNames.btn_group_toolbar,
		GroupLayout.Accordion => RootNames.accordion,
		GroupLayout.Carousel => RootNames.carousel,
		GroupLayout.Pivot => RootNames.hpvt,
		GroupLayout.Tab => RootNames.tab,
		_ => null
	};

	protected override string RootId => RootIds.group;

	protected string? Role => _layout switch
	{
		GroupLayout.Button or
			GroupLayout.HorizontalButton or
			GroupLayout.VerticalButton => RootNames.group,
		GroupLayout.ToolbarButton => RootNames.toolbar,
		_ => null
	};

	protected string? GetCssSize(ComponentSize size = ComponentSize.Medium) => _layout switch
	{
		GroupLayout.Button or
			GroupLayout.HorizontalButton or
			GroupLayout.VerticalButton or
			GroupLayout.ToolbarButton => size.ToCss(RootNames.btn_group),
		_ => null
	};
}
