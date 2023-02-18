namespace Du.Blazor.Supplement;

internal interface IDropDownToggle
{
	(int Skid, int Distance)? Offset { get; set; }
	string? Reference { get; set; }
	EventCallback OnShow { get; set; }
	EventCallback OnHide { get; set; }

	Task InternalHandleShowAsync();
	Task InternalHandleHideAsync();

	Task HideAsync();
	Task ShowAsync();
}

internal static class DropDownSupp
{
	internal static bool IsKnownRef(string? s) =>
		s is not null && (
		s.Equals("toggle", StringComparison.OrdinalIgnoreCase) || 
		s.Equals("parent", StringComparison.OrdinalIgnoreCase));

	internal static string? GetBsRef(this IDropDownToggle i) =>
		IsKnownRef(i.Reference) ? i.Reference : null;

	internal static string? GetJsRef(this IDropDownToggle i) =>
		IsKnownRef(i.Reference) ? i.Reference : null;
}
