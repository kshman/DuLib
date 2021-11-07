using System.ComponentModel;
using System.Drawing;

namespace DuLib.WinForms
{
	public class SizeMoveHitTest
	{
		[Category("SizeMoveHitTest"), Description("눌림 굵기")]
		public int Thickness { get; set; } = 10;

		[Category("SizeMoveHitTest"), Description("각 모서리 크기")]
		public int RoundLength { get; set; } = 8;

		[Category("SizeMoveHitTest"), Description("타이틀 크기")]
		public int TitleLength { get; set; } = 32;

		[Category("SizeMoveHitTest"), Description("몸뚱아리도 타이틀로 취급")]
		public bool BodyAsTitle { get; set; } = false;

		public SizeMoveHitTest()
		{
		}

		public HitTestResult HitTest(Point pt, Rectangle clientrectable)
		{
			int w = clientrectable.Width;
			int h = clientrectable.Height;

			bool t_b = pt.X > RoundLength && pt.X < w - RoundLength;
			bool l_r = pt.Y > RoundLength && pt.Y < h - RoundLength;

			bool q_t = pt.Y <= Thickness;
			bool q_b = pt.Y >= h - Thickness;
			bool q_l = pt.X <= Thickness;
			bool q_r = pt.X >= w - Thickness;

			if (q_t && t_b) return HitTestResult.Top;
			if (q_b && t_b) return HitTestResult.Bottom;
			if (q_l && l_r) return HitTestResult.Left;
			if (q_r && l_r) return HitTestResult.Right;

			if ((q_l && !l_r) && (q_t && !t_b)) return HitTestResult.TopLeft;
			if ((q_r && !l_r) && (q_t && !t_b)) return HitTestResult.TopRight;
			if ((q_l && !l_r) && (q_b && !t_b)) return HitTestResult.BottomLeft;
			if ((q_r && !l_r) && (q_b && !t_b)) return HitTestResult.BottomRight;

			if (BodyAsTitle)
				return HitTestResult.Title;
			else if (pt.Y < TitleLength)
				return HitTestResult.Title;
			else
				return HitTestResult.Body;
		}
	}

	public enum HitTestResult : int
	{
		Body = 1,
		Title = 2,
		Left = 10,
		Right = 11,
		Top = 12,
		TopLeft = 13,
		TopRight = 14,
		Bottom = 15,
		BottomLeft = 16,
		BottomRight = 17,
	}
}
