namespace Du.Blazor.Supplement;

public abstract class ElementCompose
{
	private readonly List<Func<string?>> _elements = new();
	private readonly char _separator;

	private string? _result;
	private bool _invalidate = true;

	protected ElementCompose(char separator) =>
		_separator = separator;

	public ElementCompose Add(Func<string?> element)
	{
		_elements.Add(element);
		return this;
	}

	public void Invalidate()
		=> _invalidate = true;

	public string? Result
	{
		get
		{
			if (_invalidate)
			{
				_invalidate = false;
				_result = _elements.Count == 0 ? null :
					string.Join(_separator, _elements.Select(x => x()).Where(s => s.IsHave(true)));
			}
			return _result;
		}
	}

	public override string ToString()
	{
		return $"[{_elements.Count}] {_elements}";
	}
}

public class StyleCompose : ElementCompose
{
	public StyleCompose()
		: base(';') { }
}

public class ClassCompose : ElementCompose
{
	public ClassCompose()
		 : base(' ') { }
}
