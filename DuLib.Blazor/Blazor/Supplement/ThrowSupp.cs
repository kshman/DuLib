namespace Du.Blazor.Supplement;

internal static class ThrowSupp
{
	internal static void InsideComponent(string name, string inside) =>
		throw new InvalidOperationException($"{name}: No {inside} found. ");
}
