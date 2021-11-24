using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Du.Data.Generic
{
	public class LineDb<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		protected readonly Dictionary<TKey, TValue> Db = new Dictionary<TKey, TValue>();

		public LineDb()
		{
		}

		public static LineDb<TKey, TValue> FromContext(string context, IKeyValueConverter<TKey, TValue> converter)
		{
			var l = new LineDb<TKey, TValue>();
			l.AddFromContext(context, converter);
			return l;
		}

		public static LineDb<TKey, TValue> FromFile(string filename, Encoding encoding, IKeyValueConverter<TKey, TValue> converter)
		{
			var l = new LineDb<TKey, TValue>();
			return l.AddFromFile(filename, encoding, converter) ? l : null;
		}

		public int Count => Db.Count;

		public TValue this[TKey index]
		{
			get
			{
				return Try(index, out TValue v) ? v : default;
			}
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return Db.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Db.GetEnumerator();
		}

		public void Set(TKey key, TValue value)
		{
			Db[key] = value;
		}

		public TValue Get(TKey key, TValue defvalue)
		{
			return Db.TryGetValue(key, out TValue value) ? value : defvalue;
		}

		public TValue Get(TKey key)
		{
			return Db.TryGetValue(key, out TValue value) ? value : default;
		}

		public bool Try(TKey key, out TValue value)
		{
			return Db.TryGetValue(key, out value);
		}

		public bool TryParse(TKey key, out string value)
		{
			if (!Db.TryGetValue(key, out var v))
			{
				value = null;
				return false;
			}
			else
			{
				value = v.ToString();
				return true;
			}
		}

		public bool TryParse(TKey key, out int value)
		{
			if (!Db.TryGetValue(key, out var v))
			{
				value = 0;
				return false;
			}
			else
			{
				if (!int.TryParse(v.ToString(), out value))
					return false;
				return true;
			}
		}

		public bool TryParse(TKey key, out ushort value)
		{
			if (!Db.TryGetValue(key, out var v))
			{
				value = 0;
				return false;
			}
			else
			{
				if (!ushort.TryParse(v.ToString(), out value))
					return false;
				return true;
			}
		}

		public bool Remove(TKey key)
		{
			return Db.Remove(key);
		}

		protected bool LineToKeyValue(string l, out string key, out string value)
		{
			key = null;
			value = null;

			if (l[0] == '#' || l.StartsWith("//"))
				return false;

			if (l[0] == '"')
			{
				var qt = l.IndexOf('"', 1);
				if (qt < 0)
					return false;   // no end quote. probably

				var t = l.Substring(qt + 1).TrimStart();
				if (t.Length == 0 || t[0] != '=')
					return false;   // no value

				key = l.Substring(1, qt - 1).Trim();
				value = t.Substring(1).Trim();
			}
			else
			{
				var div = l.IndexOf('=');
				if (div <= 0)
					return false;   // not valid line

				key = l.Substring(0, div).Trim();
				value = l.Substring(div + 1).Trim();
			}

			return true;
		}

		private void InternalParseLines(string ctx, IKeyValueConverter<TKey, TValue> converter)
		{
			var ss = ctx.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var v in ss)
			{
				string key, value, l = v.TrimStart();
				if (LineToKeyValue(l, out key, out value))
					Db[converter.KeyConvert(key)] = converter.ValueConvert(value);
			}
		}

		public void AddFromContext(string context, IKeyValueConverter<TKey, TValue> converter)
		{
			InternalParseLines(context, converter);
		}

		public bool AddFromFile(string filename, Encoding encoding, IKeyValueConverter<TKey, TValue> converter)
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

		public bool SaveToFile(string filename, Encoding encoding, string[] headers = null)
		{
			if (string.IsNullOrEmpty(filename))
				return false;

			using (var sw = new StreamWriter(filename, false, encoding))
			{
				if (headers != null)
				{
					foreach (var l in headers)
						sw.WriteLine($"# {l}");
					sw.WriteLine();
				}

				foreach (var l in Db)
				{
					var t = l.Key.ToString();
					if (t.IndexOf('=') != -1)
						sw.WriteLine($"\"{t}\"={l.Value}");
					else
						sw.WriteLine($"{t}={l.Value}");
				}
			}

			return true;
		}
	}
}
