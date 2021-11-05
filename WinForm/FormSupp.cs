using System.Reflection;
using System.Windows.Forms;

namespace DuLib.WinForm
{
	public static class FormSupp
	{
		public static void DoubleBuffered(Control control, bool enabled)
		{
			var prop = control.GetType().GetProperty(
				"DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

			prop.SetValue(control, enabled, null);
		}
	}
}
