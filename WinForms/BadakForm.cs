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

		#region 탑패널 - 버튼
		private void BadakCloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BadakMinButton_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void BadakMaxButton_Click(object sender, EventArgs e)
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
		#endregion

		#region 탑패널 - 마우스
		private Point _offset = new Point(0, 0);
		private bool _drag_toppanel = false;

		private void BadakTopPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_drag_toppanel = true;
				var pt = PointToScreen(e.Location);
				_offset.X = Location.X - pt.X;
				_offset.Y = Location.Y - pt.Y;
			}
			else
			{
				_drag_toppanel = false;
			}

			if (e.Clicks == 2)
			{
				_drag_toppanel = false;
				BadakMaxButton_Click(sender, e);
			}
		}

		private void BadakTopPanel_MouseUp(object sender, MouseEventArgs e)
		{
			_drag_toppanel = false;

			if (Location.Y <= 5 && WindowState != FormWindowState.Maximized)
			{
				WindowState = FormWindowState.Maximized;
				BadakMaxButton.MinMaxCloseState = MinMaxCloseState.Maximize;
			}
		}

		private void BadakTopPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (_drag_toppanel)
			{
				var pt = BadakTopPanel.PointToScreen(e.Location);
				pt.Offset(_offset);

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
	}
}
