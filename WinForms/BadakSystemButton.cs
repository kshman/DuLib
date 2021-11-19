using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Du.WinForms
{
	public partial class BadakSystemButton : UserControl
	{
		[Browsable(false)]
		public Form Form { get; set; }

		[Category("닫기 이벤트"), Description("닫기를 누르면 호출")]
		public event EventHandler CloseOrder;

		public BadakSystemButton()
		{
			InitializeComponent();
		}

		#region 프로퍼티
		[Category("버튼"), Description("내리기를 보입니다")]
		public bool ShowMinimize
		{
			get => MinButton.Visible;
			set => MinButton.Visible = value;
		}

		[Category("버튼"), Description("크게를 보입니다")]
		public bool ShowMaximize
		{
			get => MaxButton.Visible;
			set => MaxButton.Visible = value;
		}

		[Category("버튼"), Description("닫기를 보입니다")]
		public bool ShowClose
		{
			get => CloseButton.Visible;
			set => CloseButton.Visible = value;
		}
		#endregion

		#region UI 이벤트
		private void CloseButton_Click(object sender, EventArgs _)
		{
			var e = new CancelEventArgs(false);
			CloseOrder(this, e);

			if (!e.Cancel && Form != null)
				Form.Close();
		}

		private void MaxButton_Click(object sender, EventArgs _)
		{
			Maximize();
		}

		private void MinButton_Click(object sender, EventArgs _)
		{
			Minimize();
		}
		#endregion

		#region 창관리 기능
		public void Minimize()
		{
			if (Form == null)
				return;

			Form.WindowState = FormWindowState.Minimized;
		}

		public void Maximize()
		{
			if (Form == null)
				return;

			if (Form.WindowState == FormWindowState.Maximized)
			{
				Form.WindowState = FormWindowState.Normal;
				MaxButton.MinMaxCloseState = MinMaxCloseState.Normal;
			}
			else
			{
				Form.WindowState = FormWindowState.Maximized;
				MaxButton.MinMaxCloseState = MinMaxCloseState.Maximize;
			}
		}

		public void ForceNormalize()
		{
			if (Form == null)
				return;

			Form.WindowState = FormWindowState.Normal;
			MaxButton.MinMaxCloseState = MinMaxCloseState.Normal;
		}

		public void ForceMaximize()
		{
			if (Form == null)
				return;

			Form.WindowState = FormWindowState.Maximized;
			MaxButton.MinMaxCloseState = MinMaxCloseState.Maximize;
		}
		#endregion
	}
}
