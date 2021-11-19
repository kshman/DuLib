using System.Reflection;
using System.Windows.Forms;

namespace Du.WinForms
{
	public static class ControlDu
	{
		public static void DoubleBuffered(Control control, bool enabled)
		{
			var prop = control.GetType().GetProperty(
				"DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

			prop.SetValue(control, enabled, null);
		}
	}
}
