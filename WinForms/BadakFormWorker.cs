using System;
using System.Drawing;
using System.Windows.Forms;

namespace Du.WinForms
{
	public class BadakFormWorker
	{
		private Form _form;
		private BadakSystemButton _sysbtn;

		private SizeMoveHitTest _ht = new SizeMoveHitTest();

		private Point _drag_offset = new Point();
		private bool _drag_form = false;

		public BadakFormWorker(Form form)
		{
			_form = form;
			_sysbtn = null;
		}

		public BadakFormWorker(Form form, BadakSystemButton system_button)
		{
			_form = form;
			_sysbtn = system_button;
		}

		public bool BodyAsTitle => _ht.BodyAsTitle;
		public bool MoveTopToMaximize { get; set; } = true;

		private const int WM_NCHITTEST = 0x84;

		public bool WndProc(ref Message m)
		{
			if (m.Msg == WM_NCHITTEST)
			{
				var c = _form.PointToClient(Cursor.Position);
				m.Result = (IntPtr)_ht.HitTest(c, _form.ClientRectangle);
				return true;
			}

			return false;
		}

		public void Minimize()
		{
			_sysbtn?.Minimize();
		}

		public void Maximize()
		{
			_sysbtn?.Maximize();
		}

		public void DragOnDown(MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				_drag_form = false;
			else
			{
				_drag_form = true;
				var pt = _form.PointToScreen(e.Location);
				_drag_offset.X = _form.Location.X - pt.X;
				_drag_offset.Y = _form.Location.Y - pt.Y;
			}

			if (e.Clicks == 2 && _sysbtn != null)
			{
				_drag_form = false;
				_sysbtn.Maximize();
			}
		}

		public void DragOnUp(MouseEventArgs _)
		{
			_drag_form = false;

			if (_sysbtn != null && MoveTopToMaximize)
				if (_form.Location.Y <= 5 && _form.WindowState != FormWindowState.Maximized)
					_sysbtn.ForceMaximize();
		}

		public void DragOnMove(MouseEventArgs e)
		{
			if (!_drag_form)
				return;

			var pt = _form.PointToScreen(e.Location);
			pt.Offset(_drag_offset);

			if (_form.WindowState == FormWindowState.Normal)
				_form.Location = pt;
			else if (_form.WindowState == FormWindowState.Maximized)
			{
				if ((pt.X > 2 || pt.Y > 2) && _sysbtn != null)
				{
					_sysbtn.ForceNormalize();
					_form.Location = pt;
				}
			}
		}

		// end of class
	}
}
