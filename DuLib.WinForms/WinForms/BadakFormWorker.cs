namespace Du.WinForms;

/// <summary>
/// 바닥용 폼 일꾼
/// </summary>
public class BadakFormWorker
{
	private readonly Form _form;
	private readonly BadakSystemButton? _sysbtn;

	private readonly SizeMoveHitTest _ht = new();

	private Point _drag_offset;
	private bool _drag_form;

	/// <summary>
	/// 생성자
	/// </summary>
	/// <param name="form"></param>
	public BadakFormWorker(Form form)
	{
		_form = form;
		_sysbtn = null;
	}

	/// <summary>
	/// 생성자
	/// </summary>
	/// <param name="form"></param>
	/// <param name="system_button"></param>
	public BadakFormWorker(Form form, BadakSystemButton system_button)
	{
		_form = form;
		_sysbtn = system_button;
	}

	/// <summary>
	/// 몸뚱아리를 타이틀 처럼 취급(해서 이동이 가능하게)
	/// </summary>
	public bool BodyAsTitle { get => _ht.BodyAsTitle; set => _ht.BodyAsTitle = value; }
	/// <summary>
	/// 화면 윗쪽으로 가져가면 최대화한다
	/// </summary>
	public bool MoveTopToMaximize { get; set; } = true;

	private const int WM_NCHITTEST = 0x84;

	/// <summary>
	/// 윈도우 프로시저 대리자
	/// </summary>
	/// <param name="m"></param>
	/// <returns>참이면 Base를 수행할 필요가 없다. 거짓이면 수행할 것</returns>
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

	/// <summary>
	/// 최소화 하기
	/// </summary>
	public void Minimize()
	{
		_sysbtn?.Minimize();
	}

	/// <summary>
	/// 최대화/보통으로 하기
	/// </summary>
	public void Maximize()
	{
		_sysbtn?.Maximize();
	}

	/// <summary>
	/// 마우스 눌림
	/// </summary>
	/// <param name="e"></param>
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

	/// <summary>
	/// 마우스 들림
	/// </summary>
	/// <param name="_"></param>
	public void DragOnUp(MouseEventArgs _)
	{
		_drag_form = false;

		if (_sysbtn != null && MoveTopToMaximize)
			if (_form.Location.Y <= 5 && _form.WindowState != FormWindowState.Maximized)
				_sysbtn.ForceMaximize();
	}

	/// <summary>
	/// 마우스 이동
	/// </summary>
	/// <param name="e"></param>
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
}

