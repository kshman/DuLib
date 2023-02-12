using System.Collections;

namespace DuLib.Blazor.Supplement;

public abstract class ElementCompose : IEnumerable<string>
{
	private readonly List<string?> _elements = new();
	private readonly char _separator;

	protected ElementCompose(char separator) =>
		_separator = separator;

	public ElementCompose Add(string? element)
	{
		if (element.IsHave())
			_elements.Add(element);
		return this;
	}

	public ElementCompose Add(bool condition, string? element)
	{
		if (condition && element.IsHave())
			_elements.Add(element);
		return this;
	}

	public string? Result => _elements.Count == 0 ? null : string.Join(_separator, _elements);

	public IEnumerator<string> GetEnumerator()
	{
		return _elements.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _elements.GetEnumerator();
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
