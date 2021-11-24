using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Du.Data
{
	//
	public class LineStringDb<T> : Generic.LineDb<string, T>
	//, IEnumerable<KeyValuePair<string, T>>, IEnumerable, IReadOnlyCollection<KeyValuePair<string, T>>
	{
		public LineStringDb()
		{
		}

		public static LineStringDb<T> FromContext(string context, Generic.IStringConverter<T> converter)
		{
			var l = new LineStringDb<T>();
			l.AddFromContext(context, converter);
			return l;
		}

		public static LineStringDb<T> FromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
		{
			var l = new LineStringDb<T>();
			return l.AddFromFile(filename, encoding, converter) ? l : null;
		}

		private void InternalParseLines(string ctx, Generic.IStringConverter<T> converter)
		{
			var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var v in ss)
			{
				string l = v.TrimStart();
				if (LineToKeyValue(l, out string key, out string value))
					Db[key] = converter.StringConvert(value);
			}
		}

		public void AddFromContext(string context, Generic.IStringConverter<T> converter)
		{
			InternalParseLines(context, converter);
		}

		public bool AddFromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
		{
			try
			{
				if (File.Exists(filename))
				{
					var context = File.ReadAllText(filename, encoding);
					InternalParseLines(context, converter);
					return true;
				}
			}
			catch { }

			return false;
		}
	}

	//
	public class LineIntDb<T> : Generic.LineDb<int, T>
	{
		public LineIntDb()
		{
		}

		public static LineIntDb<T> FromContext(string context, Generic.IStringConverter<T> converter)
		{
			var l = new LineIntDb<T>();
			l.AddFromContext(context, converter);
			return l;
		}

		public static LineIntDb<T> FromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
		{
			var l = new LineIntDb<T>();
			return l.AddFromFile(filename, encoding, converter) ? l : null;
		}

		private void InternalParseLines(string ctx, Generic.IStringConverter<T> converter)
		{
			var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var v in ss)
			{
				string l = v.TrimStart();
				if (LineToKeyValue(l, out string key, out string value) && int.TryParse(key, out var nkey))
					Db[nkey] = converter.StringConvert(value);
			}
		}

		public void AddFromContext(string context, Generic.IStringConverter<T> converter)
		{
			InternalParseLines(context, converter);
		}

		public bool AddFromFile(string filename, Encoding encoding, Generic.IStringConverter<T> converter)
		{
			try
			{
				if (File.Exists(filename))
				{
					var context = File.ReadAllText(filename, encoding);
					InternalParseLines(context, converter);
					return true;
				}
			}
			catch { }

			return false;
		}
	}

	//
	public class LineDb : Generic.LineDb<string, string>
	{
		public LineDb()
		{
		}

		public static LineDb FromContext(string context)
		{
			var l = new LineDb();
			l.AddFromContext(context);
			return l;
		}

		public static LineDb FromFile(string filename, Encoding encoding)
		{
			var l = new LineDb();
			return l.AddFromFile(filename, encoding) ? l : null;
		}

		private void InternalParseLines(string ctx)
		{
			var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var v in ss)
			{
				string l = v.TrimStart();
				if (LineToKeyValue(l, out string key, out string value))
					Db[key] = value;
			}
		}

		public void AddFromContext(string context)
		{
			InternalParseLines(context);
		}

		public bool AddFromFile(string filename, Encoding encoding)
		{
			try
			{
				if (File.Exists(filename))
				{
					var context = File.ReadAllText(filename, encoding);
					InternalParseLines(context);
					return true;
				}
			}
			catch { }

			return false;
		}
	}

	// LineDb v3
	public class LineDbV3
	{
		protected readonly Dictionary<string, string> StringDb = new Dictionary<string, string>();
		protected readonly Dictionary<int, string> IntDb = new Dictionary<int, string>();

		protected LineDbV3()
		{
		}

		public static LineDbV3 Empty()
		{
			var l = new LineDbV3();
			return l;
		}

		public static LineDbV3 FromContext(string ctx, bool useintdb)
		{
			var l = new LineDbV3();
			l.AddFromContext(ctx, useintdb);
			return l;
		}

		public static LineDbV3 FromFile(string filename, Encoding encoding, bool useintdb)
		{
			var l = new LineDbV3();
			l.AddFromFile(filename, encoding, useintdb);
			return l;
		}

		public void AddFromContext(string context, bool useintdb = false)
		{
			ParseLines(context, useintdb);
		}

		public void AddFromFile(string filename, Encoding encoding, bool useintdb = false)
		{
			try
			{
				var context = File.ReadAllText(filename, encoding);
				ParseLines(context, useintdb);
			}
			catch { }
		}

		public bool SaveToFile(string filename, Encoding enc, string header = null)
		{
			if (string.IsNullOrEmpty(filename))
				return false;

			using (var sw = new StreamWriter(filename, false, enc))
			{
				if (!string.IsNullOrEmpty(header))
				{
					sw.WriteLine(header);
					sw.WriteLine();
				}

				foreach (var l in StringDb)
				{
					if (l.Key.IndexOf('=') < 0)
						sw.WriteLine($"{l.Key}={l.Value}");
					else
						sw.WriteLine($"\"{l.Key}\"={l.Value}");
				}

				foreach (var l in IntDb)
					sw.WriteLine($"{l.Key}={l.Value}");
			}

			return true;
		}

		private static readonly char[] _ParseSplitChars = new char[] { '\n', '\r' };

		protected void ParseLines(string ctx, bool useintdb)
		{
			StringDb.Clear();

			var ss = ctx.Split(_ParseSplitChars, StringSplitOptions.RemoveEmptyEntries);

			foreach (var v in ss)
			{
				string name, value, l = v.TrimStart();

				if (l[0] == '#' || l.StartsWith("//"))
					continue;

				if (l[0] == '"')
				{
					var qt = l.IndexOf('"', 1);
					if (qt < 0)
					{
						// no end quote?
						continue;
					}

					value = l.Substring(qt + 1).TrimStart();

					if (value.Length == 0 || value[0] != '=')
					{
						// no value
						continue;
					}

					name = l.Substring(1, qt - 1).Trim();
					value = value.Substring(1).Trim();
				}
				else
				{
					var div = l.IndexOf('=');
					if (div <= 0)
						continue;

					name = l.Substring(0, div).Trim();
					value = l.Substring(div + 1).Trim();
				}

				if (!useintdb)
					StringDb[name] = value;
				else
				{
					if (!int.TryParse(name, out var nkey))
						StringDb[name] = value;
					else
						IntDb[nkey] = value;
				}
			}
		}

		public void Set(string name, string value)
		{
			StringDb[name] = value;
		}

		public void Set(int key, string value)
		{
			IntDb[key] = value;
		}

		public string Get(string name)
		{
			return Get(name, string.Empty);
		}

		public string Get(int key)
		{
			return Get(key, string.Empty);
		}

		public string Get(string name, string defvalue)
		{
			if (!StringDb.TryGetValue(name, out string value))
				return defvalue;
			return value;
		}

		public string Get(int key, string defvalue)
		{
			if (!IntDb.TryGetValue(key, out string value))
				return defvalue;
			return value;
		}

		public bool Try(string name, out string value)
		{
			return StringDb.TryGetValue(name, out value);
		}

		public bool Try(int key, out string value)
		{
			return IntDb.TryGetValue(key, out value);
		}

		public bool Try(string name, out int value)
		{
			if (!StringDb.TryGetValue(name, out string v))
			{
				value = 0;
				return false;
			}
			else
			{
				if (!int.TryParse(v, out value))
					return false;
				return true;
			}
		}

		public bool Try(string name, out ushort value)
		{
			if (!StringDb.TryGetValue(name, out string v))
			{
				value = 0;
				return false;
			}
			else
			{
				if (!ushort.TryParse(v, out value))
					return false;
				return true;
			}
		}

		public IEnumerator<KeyValuePair<string, string>> GetStringDb()
		{
			return (IEnumerator<KeyValuePair<string, string>>)StringDb;
		}

		public IEnumerator<KeyValuePair<int, string>> GetIntDb()
		{
			return (IEnumerator<KeyValuePair<int, string>>)IntDb;
		}

		public int Count { get { return StringDb.Count + IntDb.Count; } }

		public string this[string index]
		{
			get
			{
				return Try(index, out string v) ? v : string.Empty;
			}
		}

		public string this[int key]
		{
			get
			{
				return Try(key, out string v) ? v : string.Empty;
			}
		}
	}
}
