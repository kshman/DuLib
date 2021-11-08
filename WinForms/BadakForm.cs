using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DuLib.WinForms
{
	public partial class BadakForm : Form
	{
		[Browsable(false)]
		public SizeMoveHitTest SizeMoveHitTest { get; set; } = new SizeMoveHitTest();

		public BadakForm()
		{
			InitializeComponent();
		}

		private void BasicForm_Load(object sender, EventArgs e)
		{

		}

		public override string Text { get => BadakText?.Text; set => BadakText.Text = value; }

		#region 윈도우 프로시저
		private const int WM_NCHITTEST = 0x84;

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_NCHITTEST)
			{
				var c = PointToClient(Cursor.Position);
				m.Result = (IntPtr)SizeMoveHitTest.HitTest(c, ClientRectangle);
				return;
			}

			base.WndProc(ref m);
		}
		#endregion

		#region 창관리 기능
		protected void BadakMinimize()
		{
			WindowState = FormWindowState.Minimized;
		}

		protected void BadakMaximize()
		{
			if (WindowState == FormWindowState.Maximized)
			{
				WindowState = FormWindowState.Normal;
				BadakMaxButton.MinMaxCloseState = MinMaxCloseState.Normal;
			}
			else
			{
				WindowState = FormWindowState.Maximized;
				BadakMaxButton.MinMaxCloseState = MinMaxCloseState.Maximize;
			}
		}

		private Point _drag_offset = new Point(0, 0);
		private bool _drag_window = false;

		protected void BadakDragOnMouseDown(MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				_drag_window = false;
			else
			{
				_drag_window = true;
				var pt = PointToScreen(e.Location);
				_drag_offset.X = Location.X - pt.X;
				_drag_offset.Y = Location.Y - pt.Y;
			}

			if (e.Clicks == 2)
			{
				_drag_window = false;
				BadakMaximize();
			}
		}

		protected void BadakDragOnMouseUp(MouseEventArgs e)
		{
			_drag_window = false;

			if (Location.Y <= 5 && WindowState != FormWindowState.Maximized)
			{
				WindowState = FormWindowState.Maximized;
				BadakMaxButton.MinMaxCloseState = MinMaxCloseState.Maximize;
			}
		}

		protected void BadakDragOnMouseMove(MouseEventArgs e)
		{
			if (_drag_window)
			{
				var pt = BadakTopPanel.PointToScreen(e.Location);
				pt.Offset(_drag_offset);

				if (WindowState == FormWindowState.Normal)
					Location = pt;
				else if (WindowState == FormWindowState.Maximized)
				{
					if (pt.X > 2 || pt.Y > 2)
					{
						WindowState = FormWindowState.Normal;
						BadakMaxButton.MinMaxCloseState = MinMaxCloseState.Normal;

						Location = pt;
					}
				}
			}
		}
		#endregion

		#region 탑패널 - 버튼
		private void BadakCloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BadakMinButton_Click(object sender, EventArgs e)
		{
			BadakMinimize();
		}

		private void BadakMaxButton_Click(object sender, EventArgs e)
		{
			BadakMaximize();
		}
		#endregion

		#region 탑패널 - 마우스

		private void BadakTopPanel_MouseDown(object sender, MouseEventArgs e)
		{
			BadakDragOnMouseDown(e);
		}

		private void BadakTopPanel_MouseUp(object sender, MouseEventArgs e)
		{
			BadakDragOnMouseUp(e);
		}

		private void BadakTopPanel_MouseMove(object sender, MouseEventArgs e)
		{
			BadakDragOnMouseMove(e);
		}
		#endregion
	}
}
