using Microsoft.Win32;
using System;

namespace Du.Platform
{
	public class RegKey : IDisposable
	{
		private readonly string BaseKey = "Software";

		private RegistryKey _rk;

		public RegKey(string keyname, bool createnew = false)
			: this(keyname, Registry.CurrentUser, createnew)
		{
		}

		public RegKey(string keyname, RegistryKey highkey, bool createnew = false)
		{
			OpenKey(keyname, highkey, createnew);
		}

		public RegKey(string basekey, string keyname, bool createnew = false)
			: this(basekey, keyname, Registry.CurrentUser, createnew)
		{
		}

		public RegKey(string basekey, string keyname, RegistryKey highkey, bool createnew = false)
		{
			BaseKey = basekey;
			OpenKey(keyname, highkey, createnew);
		}

		private RegKey(RegistryKey rk)
		{
			_rk = rk;
		}

		private void OpenKey(string keyname, RegistryKey highkey, bool createnew)
		{
			var key = BaseKey + "\\" + keyname;

			_rk = highkey.OpenSubKey(key, true);
			if (_rk == null && createnew)
				_rk = highkey.CreateSubKey(key);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				Close();
		}

		public void Close()
		{
			if (_rk != null)
			{
				_rk.Close();
				_rk = null;
			}
		}

		public override string ToString()
		{
			return _rk != null ? _rk.ToString() : "[closed]";
		}

		public bool IsOpen { get { return _rk != null; } }

		public RegKey CreateKey(string keyname)
		{
			return _rk == null ? null : new RegKey(_rk.CreateSubKey(keyname));
		}

		public object GetValue(string name)
		{
			return _rk?.GetValue(name);
		}

		public string GetString(string name, string failret = null)
		{
			return _rk != null && _rk.GetValue(name) is string value ? value : failret;
		}

		public int GetInt(string name, int failret = -1)
		{
			return _rk != null && _rk.GetValue(name) is int value ? value : failret;
		}

		public long GetLong(string name, long failret = -1)
		{
			return _rk != null && _rk.GetValue(name) is long value ? value : failret;
		}

		public byte[] GetBytes(string name)
		{
			return _rk != null && _rk.GetValue(name) is byte[] value ? value : null;
		}

		public string GetDecodingString(string name, string failret = null)
		{
			if (_rk != null && _rk.GetValue(name) is string value)
			{
				var s = Converter.DecodingString(value);
				if (!string.IsNullOrEmpty(s))
					return s;
			}

			return failret;
		}

		public void SetValue(string name, object value)
		{
			_rk?.SetValue(name, value);
		}

		public void SetString(string name, string value)
		{
			_rk?.SetValue(name, value, RegistryValueKind.String);
		}

		public void SetInt(string name, int value)
		{
			_rk?.SetValue(name, value, RegistryValueKind.DWord);
		}

		public void SetLong(string name, long value)
		{
			_rk?.SetValue(name, value, RegistryValueKind.QWord);
		}

		public void SetBytes(string name, byte[] value)
		{
			_rk?.SetValue(name, value, RegistryValueKind.Binary);
		}

		public void SetEncodingString(string name, string value)
		{
			if (_rk != null)
			{
				var s = Converter.EncodingString(value);
				_rk.SetValue(name, s, RegistryValueKind.String);
			}
		}

		public bool DeleteKey(string keyname, bool also_delete_tree = false)
		{
			if (_rk == null)
				return false;

			try
			{
				if (also_delete_tree)
					_rk.DeleteSubKeyTree(keyname);
				else
					_rk.DeleteSubKey(keyname);
				return true;
			}
			catch { return false; }
		}

		public bool DeleteValue(string name)
		{
			if (_rk == null)
				return false;

			try
			{
				_rk.DeleteValue(name);
				return true;
			}
			catch { return false; }
		}

		//
		public static bool RegisterExtension(string extension, string type, string description, string executepath, string friendlyname = null)
		{
			// (".testext", "Test.testext", "Test extension register", "c:\test.exe", "테스트프로그램")

			try
			{
				using (var rc = new RegKey("Classes"))
				{
					using (var re = rc.CreateKey(extension))
						re.SetString(null, type);

					using (var rt = rc.CreateKey(type)) 
					{
						rt.SetString(null, description);

						using (var rs = rt.CreateKey("shell"))
						{
							using (var ro = rs.CreateKey("open"))
							{
								if (!string.IsNullOrEmpty(friendlyname))
									rc.SetString("FriendlyAppName", friendlyname);

								using (var rn = ro.CreateKey("command"))
								{
									var command = string.Format("\"{0}\" \"%1\"", executepath);
									rn.SetString(null, command);
								}
							}
						}
					}
				}

				return true;
			}
			catch { return false; }
		}

		//
		public static bool UnregisterExtension(string extension, string type)
		{
			// (".testext", "Test.testext")

			try
			{
				using (var rc = new RegKey("Classes"))
				{
					rc.DeleteKey(extension);
					rc.DeleteKey(type, true);
				}

				return true;
			}
			catch { return false; }
		}
	}
}
