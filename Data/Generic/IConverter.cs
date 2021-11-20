namespace Du.Data.Generic
{
	public interface IConverter
	{
		object Convert(string s);
	}

	public interface IConverter<T>
	{
		T Convert(string s);
	}

	public interface IKeyValueConverter
	{
		object KeyConvert(string key);
		object ValueConvert(string value);
	}

	public interface IKeyValueConverter<TKey, TValue>
	{
		TKey KeyConvert(string key);
		TValue ValueConvert(string value);
	}
}
