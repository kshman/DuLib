namespace Du.Blazor.Components;

public class DuGroupBase : DuComponentParent
{
	/// <summary>그룹 모양. <see cref="GroupLayout"/> 참고</summary>
	[Parameter]
	public GroupLayout Layout
	{
		get => _layout;
		set
		{
			if (_layout != value)
			{
				_layout = value;
				CssClass.Invalidate();
			}
		}
	}

	private GroupLayout _layout;

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
			_ => null,
		};

	protected string? Role =>
		_layout switch
		{
			GroupLayout.Button or
			GroupLayout.HorizontalButton or
			GroupLayout.VerticalButton => RootClasses.group,
			GroupLayout.ToolbarButton => RootClasses.toolbar,
			_ => null,
		};

	protected string? GetButtonLayout(ComponentSize size = ComponentSize.Medium) =>
		_layout switch
		{
			GroupLayout.Button or
			GroupLayout.HorizontalButton or
			GroupLayout.VerticalButton or
			GroupLayout.ToolbarButton => size.ToCss(RootClasses.btn_group),
			_ => null,
		};
}
