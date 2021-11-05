using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DuLib.Data
{
	// LineDb v3
	public class LineDb
	{
		private Dictionary<string, string> _dbstr = new Dictionary<string, string>();
		private Dictionary<int, string> _dbint = new Dictionary<int, string>();

		private LineDb()
		{
		}

		public static LineDb FromContext(string ctx, bool useintdb)
		{
			var l = new LineDb();
			l.AddFromContext(ctx, useintdb);
			return l;
		}

		public static LineDb FromFile(string filename, Encoding encoding, bool useintdb)
		{
			var l = new LineDb();
			l.AddFromFile(filename, encoding, useintdb);
			return l;
		}

		public void AddFromContext(string context, bool useintdb=false)
		{
			ParseLines(context, useintdb);
		}

		public void AddFromFile(string filename, Encoding encoding, bool useintdb=false)
		{
			try
			{
				var context = File.ReadAllText(filename, encoding);
				ParseLines(context, useintdb);
			} catch { }
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

				foreach (var l in _dbstr)
				{
					if (l.Key.IndexOf('=') < 0)
						sw.WriteLine($"{l.Key}={l.Value}");
					else
						sw.WriteLine($"\"{l.Key}\"={l.Value}");
				}

				foreach (var l in _dbint)
					sw.WriteLine($"{l.Key}={l.Value}");
			}

			return true;
		}

		private static char[] _ParseSplitChars = new char[] { '\n', '\r' };

		private void ParseLines(string ctx, bool useintdb)
		{
			_dbstr.Clear();

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
				{
					if (_dbstr.TryGetValue(name, out _))
						_dbstr.Remove(name);
					_dbstr.Add(name, value);
				}
				else
				{
					if (!int.TryParse(name, out var nkey))
					{
						if (_dbstr.TryGetValue(name, out _))
							_dbstr.Remove(name);
						_dbstr.Add(name, value);
					}
					else
					{
						if (_dbint.TryGetValue(nkey, out _))
							_dbint.Remove(nkey);
						_dbint.Add(nkey, value);
					}
				}
			}
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
			if (!_dbstr.TryGetValue(name, out string value))
				return defvalue;
			return value;
		}

		public string Get(int key, string defvalue)
		{
			if (!_dbint.TryGetValue(key, out string value))
				return defvalue;
			return value;
		}

		public bool Try(string name, out string value)
		{
			return _dbstr.TryGetValue(name, out value);
		}

		public bool Try(int key, out string value)
		{
			return _dbint.TryGetValue(key, out value);
		}

		public bool Try(string name, out int value)
		{
			if (!_dbstr.TryGetValue(name, out string v))
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
			if (!_dbstr.TryGetValue(name, out string v))
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
			return (IEnumerator<KeyValuePair<string, string>>)_dbstr;
		}

		public IEnumerator<KeyValuePair<int, string>> GetIntDb()
		{
			return (IEnumerator<KeyValuePair<int, string>>)_dbint;
		}

		public int Count { get { return _dbstr.Count + _dbint.Count; } }

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
