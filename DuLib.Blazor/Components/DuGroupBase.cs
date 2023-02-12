using Microsoft.AspNetCore.Components;

namespace DuLib.Blazor;

public class DuGroupBase : DuComponentParent
{
	/// <summary>그룹 모양. <see cref="GroupLayout"/> 참고</summary>
	[Parameter] public GroupLayout Layout { get; set; }

	protected override string? RootClass =>
		Layout switch
		{
			GroupLayout.Button or
			GroupLayout.HorizontalButton => "btn-group",
			GroupLayout.VerticalButton => "btn-group-vertical",
			GroupLayout.ToolbarButton => "btn-toolbar",
			GroupLayout.Accordion => "accordion",
			GroupLayout.Carousel => "carousel slide",
			GroupLayout.Tab => "tab",
			_ => null,
		};

	protected string? Role =>
		Layout switch
		{
			GroupLayout.Button or
			GroupLayout.HorizontalButton or
			GroupLayout.VerticalButton => "group",
			GroupLayout.ToolbarButton => "toolbar",
			_ => null,
		};
}
