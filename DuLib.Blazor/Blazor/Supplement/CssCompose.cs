namespace Du.Blazor.Supplement;

/// <summary>
/// CSS 클래스 만들어 주기
/// </summary>
public class CssCompose
{
	private const char class_separator = ' ';
	private const char style_separator = ';';

	private readonly List<string> _sts = new();
	private readonly List<Func<string?>> _fns = new();

	public CssCompose Set(string className)
	{
		_sts.Add(className);
		return this;
	}

	public CssCompose Add(string? className)
	{
		if (className.IsHave())
			_sts.Add(className!);
		return this;
	}

	public CssCompose Add(bool condition, string className)
	{
		if (condition)
			_sts.Add(className);
		return this;
	}

	public CssCompose Register(Func<string?> classFunc)
	{
		_fns.Add(classFunc);
		return this;
	}

	public bool Test(string className) 
		=> _sts.Contains(className);

	private string InternalJoin(char separator)
	{
		var s = string.Join(separator, _sts);
		var f = string.Join(separator, _fns.Select(x => x()).Where(s => s.IsHave()));
		return f.Length == 0 ? s : $"{s}{separator}{f}";
	}

	public string Class =>
		InternalJoin(class_separator);

	public string Style =>
		InternalJoin(style_separator);

	public static string? Join(char sparator, params string?[] args)
	{
		var j = string.Join(sparator, args.Where(x => x.IsHave()));
		return j.Length == 0 ? null : j;
	}

	public static string? Join(params string?[] args) =>
		Join(class_separator, args);
}
