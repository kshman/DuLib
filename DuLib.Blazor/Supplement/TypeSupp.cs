using Microsoft.JSInterop;

namespace DuLib.Blazor.Supplement
{
	internal static class TypeSupp
	{
		private static int _count = 1;

		internal static int Increment => Interlocked.Increment(ref _count);

		#region 문자열

		internal static bool IsEmpty(this string? s) =>
			string.IsNullOrEmpty(s);

		internal static bool IsWhiteSpace(this string? s) =>
			string.IsNullOrWhiteSpace(s);

		internal static bool IsHave(this string? s, bool alsoTestSpace = false) =>
			alsoTestSpace ? !string.IsNullOrWhiteSpace(s) : !string.IsNullOrEmpty(s);

		internal static string? ToExtendCss(this string? value, string rootElement) =>
			value.IsHave() ? $"{rootElement}-{value}" : null;

		#endregion 문자열

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

		#endregion 컴포넌트

		#region 자바스크립트

		internal static ValueTask<IJSObjectReference> ImportModuleAsync(this IJSRuntime js, string moduleName, string? subPath)
		{
			var path = subPath.IsHave() ?
				"./_content/DuLib.Blazor/" + subPath + "/" + moduleName + ".razor.js" :
				"./_content/DuLib.Blazor/" + moduleName + ".js";
			return js.InvokeAsync<IJSObjectReference>("import", path);
		}

		#endregion
	}
}
