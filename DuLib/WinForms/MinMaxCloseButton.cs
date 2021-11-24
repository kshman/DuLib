using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Du.WinForms
{
	public class MinMaxCloseButton : Button
	{
		private Color _color_now = Color.Gray;
		private Color _color_hover = Color.FromArgb(180, 200, 240);
		private Color _color_click = Color.FromArgb(160, 180, 200);
		private Color _color_save;
		private Point _text_loc = new Point(6, -20);
		private MinMaxCloseState _mmc_state;

		[Category("MinMaxClose 버튼"), Description("최소화/최대화/최대화상태/닫기 모양 고르기")]
		public MinMaxCloseState MinMaxCloseState
		{
			get { return _mmc_state; }
			set { _mmc_state = value; Invalidate(); }
		}

		[Category("MinMaxClose 버튼"), Description("배경 색깔")]
		public Color BZBackColor
		{
			get { return _color_now; }
			set { _color_now = value; Invalidate(); }
		}

		[Category("MinMaxClose 버튼"), Description("마우스 올라감 색깔")]
		public Color MouseHoverColor
		{
			get { return _color_hover; }
			set { _color_hover = value; Invalidate(); }
		}

		[Category("MinMaxClose 버튼"), Description("마우스 누름 색깔")]
		public Color MouseClickColor
		{
			get { return _color_click; }
			set { _color_click = value; Invalidate(); }
		}

		[Category("MinMaxClose 버튼"), Description("모양 위치")]
		public Point TextLocation
		{
			get { return _text_loc; }
			set { _text_loc = value; Invalidate(); }
		}

		public MinMaxCloseButton()
		{
			Size = new Size(31, 24);
			ForeColor = Color.White;
			FlatStyle = FlatStyle.Flat;
		}

		//method mouse enter
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			_color_save = _color_now;
			_color_now = _color_hover;
		}

		//method mouse leave
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			_color_now = _color_save;
		}

		protected override void OnMouseDown(MouseEventArgs mevent)
		{
			base.OnMouseDown(mevent);
			_color_now = _color_click;
		}

		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			base.OnMouseUp(mevent);
			_color_now = _color_save;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			pe.Graphics.FillRectangle(new SolidBrush(_color_now), ClientRectangle);

			int x = _text_loc.X, y = _text_loc.Y;

			switch (_mmc_state)
			{
				case MinMaxCloseState.Normal:
					//draw and fill thw rectangles of maximized window     
					for (int i = 0; i < 2; i++)
					{
						pe.Graphics.DrawRectangle(new Pen(ForeColor), x + i + 1, y, 10, 10);
						pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 1, y - 1, 12, 4);
					}
					break;

				case MinMaxCloseState.Maximize:
					//draw and fill thw rectangles of maximized window     
					for (int i = 0; i < 2; i++)
					{
						pe.Graphics.DrawRectangle(new Pen(ForeColor), x + 5, y, 8, 8);
						pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 5, y - 1, 9, 4);

						pe.Graphics.DrawRectangle(new Pen(ForeColor), x + 2, y + 5, 8, 8);
						pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 2, y + 4, 9, 4);

					}
					break;

				case MinMaxCloseState.Minimize:
					pe.Graphics.DrawLine(new Pen(ForeColor), x + 1, y, x + 11, y);
					pe.Graphics.DrawLine(new Pen(ForeColor), x + 1, y + 1, x + 11, y + 1);
					break;

				case MinMaxCloseState.Close:
					for (int i = 0; i < 2; i++)
					{
						pe.Graphics.DrawLine(new Pen(ForeColor), x + i + 1, y, x + i + 11, y + 10);
						pe.Graphics.DrawLine(new Pen(ForeColor), x + i + 1, y + 10, x + i + 11, y);
					}
					break;
			}
		}
	}

	public enum MinMaxCloseState
	{
		Normal,
		Minimize,
		Maximize,
		Close,
	}
}
