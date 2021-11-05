using Microsoft.Win32;
using System;

namespace DuLib.System
{
	public class RegKey : IDisposable
	{
		private const string BaseKey = "Software";

		private string _key;
		private RegistryKey _rk;

		public RegKey(string keyname, bool createnew = false)
			: this(keyname, Registry.CurrentUser, createnew)
		{
		}

		public RegKey(string keyname, RegistryKey highkey, bool createnew = false)
		{
			_key = BaseKey + "\\" + keyname;

			_rk = highkey.OpenSubKey(_key, true);
			if (_rk == null && createnew)
				_rk = highkey.CreateSubKey(_key);
		}

		public void Dispose()
		{
			if (_rk != null)
				_rk.Dispose();
		}

		public void Close()
		{
			if (_rk != null)
			{
				_rk.Close();
				_rk = null;
			}
		}

		public bool IsOpen { get { return _rk != null; } }

		public string GetString(string name, string failret = null)
		{
			return _rk != null && _rk.GetValue(name) is string value ? value : failret;
		}

		public int GetInt(string name, int failret = -1)
		{
			return _rk != null && _rk.GetValue(name) is int value ? value : failret;
		}

		public byte[] GetBytes(string name)
		{
			return _rk != null && _rk.GetValue(name) is byte[] value ? value : null;
		}

		public void SetString(string name, string value)
		{
			if (_rk != null)
				_rk.SetValue(name, value, RegistryValueKind.String);
		}

		public void SetInt(string name, int value)
		{
			if (_rk != null)
				_rk.SetValue(name, value, RegistryValueKind.DWord);
		}

		public void SetLong(string name, long value)
		{
			if (_rk != null)
				_rk.SetValue(name, value, RegistryValueKind.QWord);
		}

		public void SetBytes(string name, byte[] value)
		{
			if (_rk != null)
				_rk.SetValue(name, value, RegistryValueKind.Binary);
		}
	}
}
