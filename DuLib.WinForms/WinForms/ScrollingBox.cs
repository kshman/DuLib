namespace Du.WinForms;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

/// <summary>
/// 스크롤링 박스
/// URL: https://www.codeproject.com/articles/10678/scrolling-credits-control
/// </summary>
[ToolboxBitmap("ScrollBoxToolIcon")]
public class ScrollingBox : Control
{
	private System.Timers.Timer timer;
	private ScrollingBoxCollection items;
	private ArrowDirection movingDirection;
	private bool showBackgroundImage;
	private StringFormat stringFormat;
	private int lastY;
	private bool isMouseDown;
	private bool startingPositionHasBeenSetAfterHeight;

	/// <summary>
	/// 생성자
	/// </summary>
	public ScrollingBox()
	{
		SetStyle(ControlStyles.UserPaint
			| ControlStyles.OptimizedDoubleBuffer
			| ControlStyles.AllPaintingInWmPaint, true);

		items = new ScrollingBoxCollection();
		items.OnCollectionChanged += new EventHandler(Items_OnCollectionChanged);

		movingDirection = ArrowDirection.Up;
		showBackgroundImage = false;
		stringFormat = new StringFormat();

		lastY = 0;
		isMouseDown = false;
		startingPositionHasBeenSetAfterHeight = false;

		timer = new System.Timers.Timer();
		timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
		timer.Interval = 25;

		timer.Enabled = true;
	}

	private void Items_OnCollectionChanged(object? sender, EventArgs e)
	{
		RecalculateItems();
	}

	private void RecalculateItems()
	{
		var sizeF = new SizeF();
		var g = CreateGraphics();
		var availWidth = Width - Padding.Left - Padding.Right;

		for (var i = 0; i < items.Count; i++)
		{
			var item = items[i];

			item.rectF.X = Padding.Left;
			item.rectF.Width = availWidth;

			if (item is ScrollingBoxText textItem)
			{
				sizeF = g.MeasureString(textItem.Text, Font, availWidth, stringFormat);
			}
			else if (item is ScrollingBoxImage imgItem)
			{
				sizeF = imgItem.Image.Size;
				imgItem.rectF.Width = sizeF.Width;
				switch (Alignment)
				{
					case StringAlignment.Near:
						item.rectF.X = Padding.Left;
						break;
					case StringAlignment.Center:
						item.rectF.X = (availWidth / 2) - (sizeF.Width / 2) + Padding.Left;
						break;
					case StringAlignment.Far:
						item.rectF.X = Width - sizeF.Width - Padding.Right;
						break;
				}
			}

			item.rectF.Height = sizeF.Height;

			if (i == 0)
			{
				if (!startingPositionHasBeenSetAfterHeight)
				{
					item.rectF.Y = Height;
					startingPositionHasBeenSetAfterHeight = true;
				}
			}
			else
			{
				var prev = items[i - 1];
				item.rectF.Y = (float)prev.rectF.Y + prev.rectF.Height;
			}
		}
	}

	private void PositionItems()
	{
		for (var i = 0; i < items.Count; i++)
		{
			var item = items[i];

			if (movingDirection == ArrowDirection.Up)
			{
				if (item.rectF.Y + item.rectF.Height < 0)
				{
					if (i == 0)
					{
						// Goto the bottom of the screen list items
						var last = items[items.Count - 1];
						item.rectF.Y = last.rectF.Y + Height + item.rectF.Height;
					}
					else
					{
						var prev = items[i - 1];
						item.rectF.Y = prev.rectF.Y + prev.rectF.Height;
					}
				}
				else
				{
					// Move up the screen
					item.rectF.Y -= 1;
				}
			}
			else if (movingDirection == ArrowDirection.Down)
			{
				if (item.rectF.Y > this.Height)
				{
					if (i == items.Count - 1)
					{
						// Goto the top of the screen list items
						var first = items[0];
						item.rectF.Y = first.rectF.Y - this.Height - item.rectF.Height;
					}
					else
					{
						var next = items[i + 1];
						item.rectF.Y = next.rectF.Y - item.rectF.Height;
					}
				}
				else
				{
					// Move down the screen
					item.rectF.Y += 1;
				}
			}
		}

		Invalidate();
	}

	private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
	{
		PositionItems();
	}

	/// <summary>
	/// 그리기
	/// </summary>
	/// <param name="e">그리기 인수</param>
	protected override void OnPaint(PaintEventArgs e)
	{
		var g = e.Graphics;
		var textBrush = new SolidBrush(ForeColor);
		var clipRectF = new RectangleF(
			e.ClipRectangle.X, e.ClipRectangle.Y,
			e.ClipRectangle.Width, e.ClipRectangle.Height);

		if (!showBackgroundImage)   // faster than checking if BackgroundImage is null
		{
			g.Clear(BackColor);
		}
		else
		{
			g.DrawImageUnscaled(this.BackgroundImage, 0, 0);
		}

		for (var i = 0; i < items.Count; i++)
		{
			var item = items[i];

			if (clipRectF.IntersectsWith(item.rectF))
			{
				if (item is ScrollingBoxText boxtext)
				{
					g.DrawString(boxtext.Text, Font, textBrush, item.rectF, stringFormat);
				}
				else if (item is ScrollingBoxImage boximage)
				{
					g.DrawImage(boximage.Image, item.rectF);
				}
			}
		}

		// Draw a border
		ControlPaint.DrawBorder(g, ClientRectangle, ControlPaint.Dark(BackColor), ButtonBorderStyle.Solid);
	}

	/// <summary>
	/// 배경 그리기
	/// </summary>
	/// <param name="pevent">배경 그리기 인수</param>
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
		// overridden
	}

	/// <summary>
	/// 크기 바뀔때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		RecalculateItems();
		Invalidate();
	}

	/// <summary>
	/// 글꼴 바뀔때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		RecalculateItems();
	}

	/// <summary>
	/// 배경 이미지 바뀔때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnBackgroundImageChanged(EventArgs e)
	{
		base.OnBackgroundImageChanged(e);
		showBackgroundImage = BackgroundImage != null;
	}

	/// <summary>
	/// 마우스 눌릴때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		isMouseDown = true;
		timer.Enabled = false;
		Cursor = Cursors.Hand;
	}

	/// <summary>
	/// 마우스 올라갈때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		isMouseDown = false;
		timer.Enabled = true;
		movingDirection = ArrowDirection.Up;
		Cursor = Cursors.Default;
	}

	/// <summary>
	/// 마우스 움직일때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (!DesignMode)
		{
			if (isMouseDown)
			{
				movingDirection = lastY < e.Y ? ArrowDirection.Down : ArrowDirection.Up;
				lastY = e.Y;
				PositionItems();
			}
		}
	}

	/// <summary>
	/// 패딩 바뀔때
	/// </summary>
	/// <param name="e">인수</param>
	protected override void OnPaddingChanged(EventArgs e)
	{
		base.OnPaddingChanged(e);
		RecalculateItems();
		Invalidate();
	}

	/// <summary>
	/// 텍스트
	/// </summary>
	[Browsable(false)]
	public override string Text
	{
		get => base.Text;
		set => base.Text = value;
	}

	/// <summary>
	/// 아이템 콜렉션
	/// </summary>
	[Browsable(false)]
	public ScrollingBoxCollection Items
	{
		get => items;
		set => items = value;
	}

	/// <summary>
	/// 스크롤 켬끔
	/// </summary>
	[Browsable(true), DefaultValue(true)]
	public bool ScrollEnabled
	{
		get => timer.Enabled;
		set => timer.Enabled = value;
	}

	/// <summary>
	/// 문자열 정렬
	/// </summary>
	[Browsable(true), DefaultValue(StringAlignment.Far)]
	public StringAlignment Alignment
	{
		get => stringFormat.Alignment;
		set
		{
			stringFormat.Alignment = value;
			RecalculateItems();
			Invalidate();
		}
	}
}

/// <summary>
/// 스크롤링 박스 아이템
/// </summary>
public class ScrollingBoxItem
{
	internal RectangleF rectF = new(0, 0, 0, 0);

	/// <summary>
	/// 생성자
	/// </summary>
	public ScrollingBoxItem()
	{
	}
}

internal class ScrollingBoxText : ScrollingBoxItem
{
	private string text;

	public ScrollingBoxText(string Text)
		: base()
	{
		text = Text;
	}

	public string Text
	{
		get => text;
		set => text = value;
	}
}

internal class ScrollingBoxImage : ScrollingBoxItem
{
	private Image img;

	public ScrollingBoxImage(Image Image)
		: base()
	{
		img = Image;
	}

	public Image Image
	{
		get => img;
		set => img = value;
	}
}

/// <summary>
/// 스크롤링 박스 아이템 콜렉션
/// </summary>
public class ScrollingBoxCollection : System.Collections.CollectionBase
{
	/// <summary>
	/// 콜렉션에 바뀜이 있을때
	/// </summary>
	public event EventHandler? OnCollectionChanged;

	/// <summary>
	/// 생성자
	/// </summary>
	public ScrollingBoxCollection()
	{
	}

	/// <summary>
	/// 아이템 추가
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public int Add(ScrollingBoxItem value)
	{
		int index = InnerList.Add(value);
		OnCollectionChanged?.Invoke(this, new EventArgs());
		return index;
	}

	/// <summary>
	/// 문장 추가
	/// </summary>
	/// <param name="Text"></param>
	/// <returns></returns>
	public int Add(string Text)
	{
		return Add(new ScrollingBoxText(Text));
	}

	/// <summary>
	/// 이미지 추가
	/// </summary>
	/// <param name="image"></param>
	/// <returns></returns>
	public int Add(Image image)
	{
		return Add(new ScrollingBoxImage(image));
	}

	/// <summary>
	/// 아이템 껴넣기
	/// </summary>
	/// <param name="index"></param>
	/// <param name="value"></param>
	public void InsertAt(int index, ScrollingBoxItem value)
	{
		InnerList.Insert(index, value);
		OnCollectionChanged?.Invoke(this, new EventArgs());
	}

	/// <summary>
	/// 아이템 삭제
	/// </summary>
	/// <param name="value"></param>
	public void Remove(ScrollingBoxItem value)
	{
		InnerList.Remove(value);
		OnCollectionChanged?.Invoke(this, new EventArgs());
	}

	/// <summary>
	/// 아이템 인덱싱
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public ScrollingBoxItem? this[int index]
	{
		get => InnerList[index] as ScrollingBoxItem;
		set => InnerList[index] = value;
	}
}

#pragma warning restore CS8602 // Dereference of a possibly null reference.
