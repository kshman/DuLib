namespace Du.WinForms;


/// <summary>
/// 드롭 메뉴가 달린 버튼
/// https://stackoverflow.com/questions/10803184/windows-forms-button-with-drop-down-menu/10803307
/// </summary>
public class SplitButton : Button
{
	/// <summary>
	/// 드롭 메뉴
	/// </summary>
	[DefaultValue(null), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public ContextMenuStrip? Menu { get; set; }

	/// <summary>
	/// 드롭 선택 너비
	/// </summary>
	[DefaultValue(20), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public int SplitWidth { get; set; } = 20;

	/// <summary>
	/// 생성자
	/// </summary>
	public SplitButton()
	{
		BackColor = Color.FromArgb(20, 20, 20);
		ForeColor = Color.White;
	}

	/// <summary>
	/// 마우스 눌림
	/// </summary>
	/// <param name="mevent"></param>
	protected override void OnMouseDown(MouseEventArgs mevent)
	{
		var splitRect = new Rectangle(Width - SplitWidth, 0, SplitWidth, Height);

		// Figure out if the button click was on the button itself or the menu split
		if (Menu != null &&
			mevent.Button == MouseButtons.Left &&
			splitRect.Contains(mevent.Location))
		{
#if true
			Menu.Show(this, 0, Height);             // Shows menu under button
#else
			Menu.Show(this, mevent.Location);		// Shows menu at click location  
#endif
		}
		else
		{
			base.OnMouseDown(mevent);
		}
	}

	/// <summary>
	/// 컨트롤 그리기
	/// </summary>
	/// <param name="pevent"></param>
	protected override void OnPaint(PaintEventArgs pevent)
	{
		base.OnPaint(pevent);

		if (Menu != null && SplitWidth > 0)
		{
			// Draw the arrow glyph on the right side of the button
			int arrowX = ClientRectangle.Width - 14;
			int arrowY = ClientRectangle.Height / 2 - 1;

			var arrowColor = Enabled ? ForeColor : SystemColors.ButtonShadow;
			//var arrowBrush = Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow;
			var arrowBrush = new SolidBrush(arrowColor);
			var arrows = new[] {
					new Point(arrowX, arrowY),
					new Point(arrowX + 7, arrowY),
					new Point(arrowX + 3, arrowY + 4)
				};
			pevent.Graphics.FillPolygon(arrowBrush, arrows);

			// Draw a dashed separator on the left of the arrow
			int lineX = ClientRectangle.Width - SplitWidth;
			int lineYFrom = arrowY - 4;
			int lineYTo = arrowY + 8;
			using var separatorPen = new Pen(Brushes.DarkGray) 
			{
				DashStyle = System.Drawing.Drawing2D.DashStyle.Dot 
			};
			pevent.Graphics.DrawLine(separatorPen, lineX, lineYFrom, lineX, lineYTo);
		}
	}
}
