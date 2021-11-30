namespace Du.WinForms;

/// <summary>
/// 상태 모양 버튼
/// </summary>
public class BadakStateButton : Button
{
	private Color _color_now = Color.Gray;
	private Color _color_hover = Color.FromArgb(180, 200, 240);
	private Color _color_click = Color.FromArgb(160, 180, 200);
	private Color _color_save;
	private Point _shape_location = new(6, -20);
	private BadakState _badak_state;

	/// <summary>
	/// 최소화/최대화/최대화상태/닫기 모양 고르기
	/// </summary>
	[Category("BadakState 버튼"), Description("최소화/최대화/최대화상태/닫기 모양 고르기")]
	public BadakState BadakState
	{
		get => _badak_state;
		set { _badak_state = value; Invalidate(); }
	}

	/// <summary>
	/// 배경 색깔
	/// </summary>
	[Category("BadakState 버튼"), Description("배경 색깔")]
	public Color BadakBackColor
	{
		get => _color_now;
		set { _color_now = value; Invalidate(); }
	}

	/// <summary>
	/// 마우스 올라감 색깔
	/// </summary>
	[Category("BadakState 버튼"), Description("마우스 올라감 색깔")]
	public Color MouseHoverColor
	{
		get => _color_hover;
		set { _color_hover = value; Invalidate(); }
	}

	/// <summary>
	/// 마우스 누름 색깔
	/// </summary>
	[Category("BadakState 버튼"), Description("마우스 누름 색깔")]
	public Color MouseClickColor
	{
		get => _color_click;
		set { _color_click = value; Invalidate(); }
	}

	/// <summary>
	/// 모양 위치
	/// </summary>
	[Category("BadakState 버튼"), Description("모양 위치")]
	public Point ShapeLocation
	{
		get => _shape_location;
		set { _shape_location = value; Invalidate(); }
	}

	/// <summary>
	/// 생성자
	/// </summary>
	public BadakStateButton()
	{
		Size = new Size(31, 24);
		ForeColor = Color.White;
		FlatStyle = FlatStyle.Flat;
		TabStop = false;
	}

	/// <summary>
	/// 마우스가 올려짐
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseEnter(EventArgs e)
	{
		base.OnMouseEnter(e);
		_color_save = _color_now;
		_color_now = _color_hover;
	}

	/// <summary>
	/// 마우스가 떠남
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
		_color_now = _color_save;
	}

	/// <summary>
	/// 마우스 눌림
	/// </summary>
	/// <param name="mevent"></param>
	protected override void OnMouseDown(MouseEventArgs mevent)
	{
		base.OnMouseDown(mevent);
		_color_now = _color_click;
	}

	/// <summary>
	/// 마우스 올라감
	/// </summary>
	/// <param name="mevent"></param>
	protected override void OnMouseUp(MouseEventArgs mevent)
	{
		base.OnMouseUp(mevent);
		_color_now = _color_save;
	}

	/// <summary>
	/// 그리기
	/// </summary>
	/// <param name="pe"></param>
	protected override void OnPaint(PaintEventArgs pe)
	{
		base.OnPaint(pe);

		pe.Graphics.FillRectangle(new SolidBrush(_color_now), ClientRectangle);

		int x = _shape_location.X, y = _shape_location.Y;

		switch (_badak_state)
		{
			case BadakState.Normal:
				for (var i = 0; i < 2; i++)
				{
					pe.Graphics.DrawRectangle(new Pen(ForeColor), x + i + 1, y, 10, 10);
					pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 1, y - 1, 12, 4);
				}
				break;

			case BadakState.Maximize:
				for (var i = 0; i < 2; i++)
				{
					pe.Graphics.DrawRectangle(new Pen(ForeColor), x + 5, y, 8, 8);
					pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 5, y - 1, 9, 4);

					pe.Graphics.DrawRectangle(new Pen(ForeColor), x + 2, y + 5, 8, 8);
					pe.Graphics.FillRectangle(new SolidBrush(ForeColor), x + 2, y + 4, 9, 4);

				}
				break;

			case BadakState.Minimize:
				pe.Graphics.DrawLine(new Pen(ForeColor), x + 1, y, x + 11, y);
				pe.Graphics.DrawLine(new Pen(ForeColor), x + 1, y + 1, x + 11, y + 1);
				break;

			case BadakState.Close:
				for (var i = 0; i < 2; i++)
				{
					pe.Graphics.DrawLine(new Pen(ForeColor), x + i + 1, y, x + i + 11, y + 10);
					pe.Graphics.DrawLine(new Pen(ForeColor), x + i + 1, y + 10, x + i + 11, y);
				}
				break;
		}
	}
}

/// <summary>
/// 바닥 상태
/// </summary>
public enum BadakState
{
	/// <summary>
	/// 보통
	/// </summary>
	Normal,
	/// <summary>
	/// 최소화
	/// </summary>
	Minimize,
	/// <summary>
	/// 최대화
	/// </summary>
	Maximize,
	/// <summary>
	/// 닫기
	/// </summary>
	Close,
}
