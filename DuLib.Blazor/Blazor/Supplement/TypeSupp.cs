namespace Du.Blazor.Supplement
{
	internal static class TypeSupp
	{
		private static int _count = 1;

		internal static int Increment => Interlocked.Increment(ref _count);

		#region 기본 타입

		internal static bool IsEmpty(this string? s) =>
			string.IsNullOrEmpty(s);

		internal static bool IsWhiteSpace(this string? s) =>
			string.IsNullOrWhiteSpace(s);

		internal static bool IsHave(this string? s, bool alsoTestSpace = false) =>
			alsoTestSpace ? !string.IsNullOrWhiteSpace(s) : !string.IsNullOrEmpty(s);

		internal static string? ToExtendCss(this string? value, string rootElement) =>
			value.IsHave() ? $"{rootElement}-{value}" : null;

		internal static string? IfTrue(this bool condition, string? value) =>
			condition ? value : null;

		internal static string? IfFalse(this bool condition, string? value) =>
			condition ? null : value;

		#endregion 기본 타입

		#region 컴포넌트

		internal static string? ToCss(this ComponentVisibility visibility) =>
			visibility == ComponentVisibility.Hidden ? "visibility:hidden" :
			visibility == ComponentVisibility.Collapsed ? "display:none" : null;

		internal static string? ToCss(this ComponentSize size, string lead)
		{
			switch (size)
			{
				case ComponentSize.Small:
					return $"{lead}-sm";

				case ComponentSize.Large:
					return $"{lead}-lg";

				case ComponentSize.Medium:
				default:
					break;
			}
			return null;
		}

		internal static string ToCss(this ComponentColor color, string lead)
		{
			var cs = color switch
			{
				ComponentColor.Primary => "primary",
				ComponentColor.Secondary => "secondary",
				ComponentColor.Success => "success",
				ComponentColor.Danger => "danger",
				ComponentColor.Warning => "warning",
				ComponentColor.Info => "info",
				ComponentColor.Light => "light",
				ComponentColor.Dark => "dark",
				_ => "dark",
			};
			return $"{lead}-${cs}";
		}

		#endregion 컴포넌트

		#region 자바스크립트

		internal static ValueTask<IJSObjectReference> ImportModuleAsync(this IJSRuntime js, string moduleName, string? subPath)
		{
			var path = subPath.IsHave() ?
				"./_content/Du.Blazor/" + subPath + "/" + moduleName + ".razor.js" :
				"./_content/Du.Blazor/" + moduleName + ".js";
			return js.InvokeAsync<IJSObjectReference>("import", path);
		}

		#endregion
	}
}
