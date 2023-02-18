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

		internal static string? ToCss(this string? value, string rootElement) =>
			value.IsHave() ? $"{rootElement}-{value}" : null;

		internal static string ToBootStrap(this bool value) =>
			value ? "true" : "false";

		internal static string? IfTrue(this bool condition, string? value) =>
			condition ? value : null;

		internal static string? IfFalse(this bool condition, string? value) =>
			condition ? null : value;

		internal static bool ShouldAwaitTask(this Task task) =>
			task.Status is not TaskStatus.RanToCompletion and not TaskStatus.Canceled;

		#endregion 기본 타입

		#region 컴포넌트

		internal static string? ToCss(this ComponentSize size, string lead) => size switch
		{
			ComponentSize.Small => $"{lead}-sm",
			ComponentSize.Large => $"{lead}-lg",
			//ComponentSize.Medium
			_ => null,
		};

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
				_ => "warning", // 알 수 없으면 경고지!
			};
			return $"{lead}-{cs}";
		}

		internal static string ToButtonCss(this ComponentColor color, bool outline) => color switch
		{
			ComponentColor.Link => "btn-link",
			_ => color.ToCss(outline ? "btn-outline" : "btn"),
		};

		internal static string ToHtml(this ButtonType? button) => button switch
		{
			//ButtonType.Button => "button",
			ButtonType.Submit => "submit",
			ButtonType.Reset => "reset",
			_ => "button",
		};

		internal static string? ToCss(this GroupLayout layout) => layout switch
		{
			GroupLayout.Button or 
				GroupLayout.Vertical => "group",
			GroupLayout.Toolbar => "toolbar",
			_ => null,
		};

		internal static string ToCss(this DropDirection dir) => dir switch
		{
			//DropDirection.Down => "dropdown",
			DropDirection.Up => "dropup",
			DropDirection.Start => "dropstart",
			DropDirection.End => "dropend",
			_ => "dropdown",
		};

		internal static string? ToCss(this DropAutoClose close) => close switch
		{
			DropAutoClose.True => "true",
			DropAutoClose.False => "false",
			DropAutoClose.Inside => "inside",
			DropAutoClose.Outside => "outside",
			_ => null,
		};

		internal static string? ToCss(this DropAlignment alignment) => alignment switch
		{
			DropAlignment.Start => "dropdown-menu-start",
			DropAlignment.End => "dropdown-menu-end",
			_ => null
		};

	#endregion 컴포넌트

	#region 자바스크립트

	// 모듈 임포트
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
