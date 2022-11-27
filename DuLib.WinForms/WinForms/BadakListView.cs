#pragma warning disable IDE1006 // 명명 스타일
using System.Runtime.InteropServices;

namespace Du.WinForms;

/// <summary>
/// 바닥 리스트뷰
/// 아이템 순서 바꾸기: https://stackoverflow.com/a/65567409
/// 서브아이템 편집: https://www.codeproject.com/Articles/6646/In-place-Editing-of-ListView-subitems
/// </summary>
public class BadakListView : ListView
{
	private const string c_reorder_mesg = "(줄 위치 바꾸기)";

	private const int LVM_FIRST = 0x1000;
	private const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

	private const int HDN_FIRST = -300;
	private const int HDN_BEGINDRAG = (HDN_FIRST - 10);
	private const int HDN_ITEMCHANGINGA = (HDN_FIRST - 0);
	private const int HDN_ITEMCHANGINGW = (HDN_FIRST - 20);

	private bool _item_reorder;

	private Control? _edit_control;
	private ListViewItem? _edit_item;
	private int _edit_sub_index;
	private Dictionary<SubItemEditCollection, Control?> _edit_collection = new();

	/// <summary>
	/// 아이템 순서를 바꿨을때
	/// </summary>
	[Description("아이템 순서를 바꾸면 호출합니다")]
	public event ItemReorderDragHandler? ItemReordered;

	/// <summary>
	/// 서브 아이템 누름
	/// </summary>
	public event SubItemEventHandler? SubItemClick;
	/// <summary>
	/// 서브 아이템 편집 시작
	/// </summary>
	public event SubItemEventHandler? SubItemBeginEdit;
	/// <summary>
	/// 서브 아이템 편짐 끝
	/// </summary>
	public event SubItemEndEditingEventHandler? SubItemEndEdit;

	/// <summary>
	/// 아이템 순서 바꾸는 기능을 켜고 끕니다
	/// </summary>
	[DefaultValue(false), Browsable(true),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
	 Description("아이템 순서를 끌어다 바꾸는 기능을 켜고 끕니다")]
	public bool AllowItemReorder
	{
		get => _item_reorder;
		set
		{
			if (value)
				base.Sorting = SortOrder.None;

			_item_reorder = value;
			AllowDrop = value;
		}
	}

	/// <summary>
	/// 아이템 순서 바꿀때 원래 아이템 인덱스
	/// </summary>
	[DefaultValue(-1), Browsable(false),
	 Description("줄 끌어다 바꿀때 원래 아이템 인덱스")]
	public int ReorderBeforeIndex { get; set; } = -1;

	/// <summary>
	/// 아이템 순서 바꿀때 바꾼 다음 아이템 인덱스
	/// </summary>
	[DefaultValue(-1), Browsable(false),
	 Description("줄 끌어다 바꿀때 바꾼 다음 아이템 인덱스")]
	public int ReorderAfterIndex { get; set; } = -1;

	/// <summary>
	/// 자동 정렬
	/// </summary>
	[DefaultValue(SortOrder.None), Browsable(true),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
	 Description("정렬 기능을 설정합니다. 단 아이템 순서를 바꿀 때는 사용할 수 없어요")]
	public new SortOrder Sorting
	{
		get => _item_reorder ? SortOrder.None : base.Sorting;
		set => base.Sorting = _item_reorder ? SortOrder.None : value;
	}

	/// <summary>
	/// 더블 클릭으로 서브 아이템을 편집할 수 있어요
	/// </summary>
	[DefaultValue(true), Browsable(true),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
	 Description("더블 클릭으로 서브 아이템을 편집할 수 있어요")]
	public bool DoubleClickToEdit { get; set; } = true;

	/// <summary>
	/// 컨스트럭터
	/// </summary>
	public BadakListView()
	{
		ListViewItemSorter = new IndexComparer();
	}

	/// <summary>
	/// 아이템을 끌 때 처리해요
	/// </summary>
	/// <param name="e"></param>
	protected override void OnItemDrag(ItemDragEventArgs e)
	{
		base.OnItemDrag(e);

		if (!_item_reorder)
			return;

		if (e.Item is not ListViewItem item)
		{
			DoDragDrop(c_reorder_mesg, DragDropEffects.Move);
			return;
		}

		ReorderBeforeIndex = Items.IndexOf(item);
		DoDragDrop(e.Item, DragDropEffects.Move);
	}

	/// <summary>
	/// 드래그를 시작할 때 처리해요
	/// </summary>
	/// <param name="drgevent"></param>
	protected override void OnDragEnter(DragEventArgs drgevent)
	{
		base.OnDragEnter(drgevent);

		if (!_item_reorder)
		{
			drgevent.Effect = DragDropEffects.None;
			return;
		}

		drgevent.Effect = drgevent.AllowedEffect;
	}

	/// <summary>
	/// 드래그가 끝날 때 처리해요
	/// </summary>
	/// <param name="e"></param>
	protected override void OnDragLeave(EventArgs e)
	{
		base.OnDragLeave(e);

		if (!_item_reorder)
			return;

		InsertionMark.Index = -1;
	}

	/// <summary>
	/// 드래그를 움직일 때 처리해요
	/// </summary>
	/// <param name="drgevent"></param>
	protected override void OnDragOver(DragEventArgs drgevent)
	{
		base.OnDragOver(drgevent);

		if (!_item_reorder)
		{
			drgevent.Effect = DragDropEffects.None;
			return;
		}

		var target = PointToClient(new Point(drgevent.X, drgevent.Y));
		var index = InsertionMark.NearestIndex(target);

		if (index >= 0)
		{
			var bound = GetItemRect(index);
			InsertionMark.AppearsAfterItem = target.X > bound.Left + (bound.Width / 2);
		}

		InsertionMark.Index = index;
	}

	/// <summary>
	/// 드래그를 놓을 때 처리해요
	/// </summary>
	/// <param name="drgevent"></param>
	protected override void OnDragDrop(DragEventArgs drgevent)
	{
		base.OnDragDrop(drgevent);

		if (!_item_reorder)
			return;

		var index = InsertionMark.Index;
		if (index < 0)
			return;

		if (InsertionMark.AppearsAfterItem)
			index++;

		if (drgevent.Data?.GetData(typeof(ListViewItem)) is not ListViewItem item)
			return;

		Items.Insert(index, (ListViewItem)item.Clone());
		Items.Remove(item);

		ReorderAfterIndex = index;

		ItemReordered?.Invoke(this,
			new ItemReorderDragEventArgs(drgevent, ReorderBeforeIndex, ReorderAfterIndex));
	}

	//
	private int[] InternalGetColumnOrder()
	{
		var cnt = Columns.Count;
		var lParam = Marshal.AllocHGlobal(Marshal.SizeOf<int>() * cnt);

		try
		{
			var res = NativeWin32.SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(cnt), lParam);
			if (res.ToInt32() == 0)
				return Array.Empty<int>();

			var order = new int[cnt];
			Marshal.Copy(lParam, order, 0, cnt);

			return order;
		}
		finally
		{
			Marshal.FreeHGlobal(lParam);
		}
	}

	/// <summary>
	/// 서브 아이템 순번을 찾아요
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="item"></param>
	/// <returns></returns>
	private int GetSubItemAt(int x, int y, out ListViewItem? item)
	{
		item = GetItemAt(x, y);

		if (item == null)
			return -1;

		var order = InternalGetColumnOrder();
		var bound = item.GetBounds(ItemBoundsPortion.Entire);
		var sx = bound.Left;

		foreach (var o in order)
		{
			var h = Columns[o];
			if (x < sx + h.Width)
				return h.Index;
			sx += h.Width;
		}

		return -1;
	}

	/// <summary>
	/// 서브 아이템 바운드를 찾아요
	/// </summary>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	/// <returns></returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	private Rectangle GetSubItemBounds(ListViewItem item, int subItemIndex)
	{
		var order = InternalGetColumnOrder();

		if (subItemIndex < 0 || subItemIndex >= order.Length)
			throw new IndexOutOfRangeException(nameof(subItemIndex));

		var bound = item.GetBounds(ItemBoundsPortion.Entire);
		var sx = bound.Left;

		foreach (var o in order)
		{
			var h = Columns[o];
			if (h.Index == subItemIndex)
				return new Rectangle(sx, bound.Top, h.Width, bound.Height);
			sx += h.Width;
		}

		return Rectangle.Empty;
	}

	/// <summary>
	/// WndProc 오버라이드
	/// </summary>
	/// <param name="m"></param>
	protected override void WndProc(ref Message m)
	{
		switch (m.Msg)
		{
			case NativeWin32.WM_VSCROLL:
			case NativeWin32.WM_HSCROLL:
			case NativeWin32.WM_SIZE:
				InternalEndEditSubItem(false);
				break;

			case NativeWin32.WM_NOTIFY:
				var hdr = Marshal.PtrToStructure<NmHdr>(m.LParam);
				if (hdr.Code is HDN_BEGINDRAG or HDN_ITEMCHANGINGA or HDN_ITEMCHANGINGW)
					InternalEndEditSubItem(false);
				break;
		}

		base.WndProc(ref m);
	}

	/// <summary>
	/// OnMouseUp 오버라이드
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);

		if (!DoubleClickToEdit)
			InternalEditSubItemAt(new Point(e.X, e.Y));
	}

	/// <summary>
	/// OnDoubleClick 오버라이드
	/// </summary>
	/// <param name="e"></param>
	protected override void OnDoubleClick(EventArgs e)
	{
		base.OnDoubleClick(e);

		if (DoubleClickToEdit)
		{
			var pt = PointToClient(Cursor.Position);
			InternalEditSubItemAt(pt);
		}
	}

	/// <summary>
	/// 서브 아이템을 편집하려고 해요
	/// </summary>
	/// <param name="e"></param>
	protected void OnSubItemBeginEdit(SubItemEventArgs e)
		=> SubItemBeginEdit?.Invoke(this, e);

	/// <summary>
	/// 서브 아이템 편집이 끝낫어요
	/// </summary>
	/// <param name="e"></param>
	protected void OnSubItemEndEdit(SubItemEndEditEventArgs e)
		=> SubItemEndEdit?.Invoke(this, e);

	/// <summary>
	/// 서브 아이템을 눌렀어요 
	/// </summary>
	/// <param name="e"></param>
	protected void OnSubItemClick(SubItemEventArgs e)
		=> SubItemClick?.Invoke(this, e);

	private void InternalEditSubItemAt(Point pt)
	{
		var index = GetSubItemAt(pt.X, pt.Y, out var item);
		if (index < 0 || item == null)
			return;
		OnSubItemClick(new SubItemEventArgs(item, index));
	}

	/// <summary>
	/// 서브 아이템을 편집할껌미다
	/// </summary>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	public void EditSubItem(ListViewItem item, int subItemIndex)
		=> EditSubItem(SubItemEditCollection.TextBox, item, subItemIndex);

	/// <summary>
	/// 서브 아이템을 편집할껀데, 컬렉션을 골라서
	/// </summary>
	/// <param name="collection"></param>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	/// <param name="collectionArgument"></param>
	public void EditSubItem(SubItemEditCollection collection, ListViewItem item, int subItemIndex, object? collectionArgument = null)
	{
		if (!_edit_collection.TryGetValue(collection, out var ctrl))
		{
			ctrl = collection switch
			{
				SubItemEditCollection.TextBox => new TextBox()
				{
					BorderStyle = BorderStyle.FixedSingle,
					Multiline = false,
					Name = "BadakListView_Collection_TextBox",
					Size = new Size(100, 16),
				},
				SubItemEditCollection.PasswordTextBox => new TextBox()
				{
					BorderStyle = BorderStyle.FixedSingle,
					Multiline = false,
					Name = "BadakListView_Collection_PasswordTextBox",
					PasswordChar = '●',
					Size = new Size(100, 16),
				},
				SubItemEditCollection.DateTime => new DateTimePicker()
				{
					Format = DateTimePickerFormat.Short,
					Name = "BadakListView_Collection_DateTime",
					Size = new Size(100, 16),
				},
				SubItemEditCollection.ComboBox => new ComboBox()
				{
					DropDownStyle = ComboBoxStyle.DropDownList,
					IntegralHeight = false,
					ItemHeight = 13,
					Name = "BadakListView_Collection_ComboBox",
					Size = new Size(100, 50),
				},
				_ => null,
			};

			if (ctrl != null)
			{
				ctrl.Text = "";
				ctrl.Visible = false;
				ctrl.Location = Point.Empty;

				Controls.Add(ctrl);
				_edit_collection.Add(collection, ctrl);
			}
		}

		if (ctrl == null)
			return;

		if (collection == SubItemEditCollection.ComboBox)
		{
			// 컴보 박스는 아이템 넣어야함
			if (collectionArgument is not string[] ss)
				return;
			var cbx = (ComboBox)ctrl;
			cbx.Items.Clear();
			cbx.Items.AddRange(ss);
		}

		EditSubItem(ctrl, item, subItemIndex);
	}

	/// <summary>
	/// 서브 아이템을 편집할꺼예여
	/// </summary>
	/// <param name="control"></param>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	public void EditSubItem(Control control, ListViewItem item, int subItemIndex)
	{
		OnSubItemBeginEdit(new SubItemEventArgs(item, subItemIndex));

		var rt = GetSubItemBounds(item, subItemIndex);
		if (rt.X < 0)
		{
			rt.Width += rt.X;
			rt.X = 0;
		}
		if (rt.X + rt.Width > Width)
			rt.Width = Width - rt.Left;
		rt.Offset(Left, Top);

		var origin = Point.Empty;
		var lv_origin = Parent?.PointToScreen(origin) ?? Point.Empty;
		var ctl_origin = control.Parent?.PointToScreen(origin) ?? Point.Empty;
		rt.Offset(lv_origin.X - ctl_origin.X, lv_origin.Y - ctl_origin.Y);

		control.Bounds = rt;
		control.Text = item.SubItems[subItemIndex].Text;
		control.Visible = true;
		control.BringToFront();
		control.Focus();

		_edit_control = control;
		_edit_control.Leave += _edit_control_Leave;
		_edit_control.KeyPress += _edit_control_KeyPress;

		_edit_item = item;
		_edit_sub_index = subItemIndex;
	}

	private void _edit_control_Leave(object? sender, EventArgs e)
		=> InternalEndEditSubItem(true);

	private void _edit_control_KeyPress(object? sender, KeyPressEventArgs e)
	{
		switch (e.KeyChar)
		{
			case (char)(int)Keys.Escape:
				InternalEndEditSubItem(false);
				break;

			case (char)(int)Keys.Enter:
				InternalEndEditSubItem(true);
				break;
		}
	}

	/// <summary>
	/// 서브 아이템 편집을 끝내자
	/// </summary>
	/// <param name="acceptChanges"></param>
	private void InternalEndEditSubItem(bool acceptChanges)
	{
		if (_edit_control == null || _edit_item == null || _edit_sub_index < 0)
			return;

		var sub = _edit_item.SubItems[_edit_sub_index];
		var text = acceptChanges ? _edit_control.Text : sub.Text;
		sub.Text = text;

		_edit_control.Leave -= _edit_control_Leave;
		_edit_control.KeyPress -= _edit_control_KeyPress;
		_edit_control.Visible = false;

		OnSubItemEndEdit(new SubItemEndEditEventArgs(
			_edit_item, _edit_sub_index, text, !acceptChanges));

		_edit_control = null;
		_edit_item = null;
		_edit_sub_index = -1;
	}

	//
	private class IndexComparer : System.Collections.IComparer
	{
		public int Compare(object? x, object? y)
		{
			if (x is ListViewItem l &&
				y is ListViewItem r)
				return l.Index - r.Index;
			return 0;
		}
	}

	//
	internal struct NmHdr
	{
#pragma warning disable CS0649
		public IntPtr _;
		public int Id;
		public int Code;
#pragma warning restore CS0649
	}
}

/// <summary>
/// 아이템 순서 바꾸기 핸들러
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void ItemReorderDragHandler(object? sender, ItemReorderDragEventArgs e);

/// <summary>
/// 아이템 순서 바꾸기 기능을 지원합니다.
/// <see cref="BadakListView.AllowItemReorder"/>나
/// </summary>
public class ItemReorderDragEventArgs : DragEventArgs
{
	/// <summary>
	/// 컨스트럴터
	/// </summary>
	/// <param name="de"></param>
	/// <param name="before"></param>
	/// <param name="after"></param>
	public ItemReorderDragEventArgs(DragEventArgs de, int before, int after)
	: base(
		de.Data,
		de.KeyState,
		de.X,
		de.Y,
		de.AllowedEffect,
		de.Effect,
		de.DropImageType,
		de.Message ?? string.Empty,
		de.MessageReplacementToken ?? string.Empty)
	{
		BeforeIndex = before;
		AfterIndex = after;
	}

	/// <summary>
	/// 아이템 순서 바꿀때 원래 아이템 인덱스
	/// </summary>
	public int BeforeIndex { get; }

	/// <summary>
	/// 아이템 순서 바꿀때 바꾼 다음 아이템 인덱스
	/// </summary>
	public int AfterIndex { get; }

	internal ItemReorderDragEventArgs Clone()
	{
		return (ItemReorderDragEventArgs)MemberwiseClone();
	}

	internal bool Equals(ItemReorderDragEventArgs? other)
	{
		if (other == this)
			return true;

		return other?.Data != null
			   && other.Data.Equals(Data)
			   && other.KeyState == KeyState
			   && other.X == X
			   && other.Y == Y
			   && other.AllowedEffect == AllowedEffect
			   && other.Effect == Effect
			   && other.DropImageType == DropImageType
			   && other.Message == Message
			   && other.MessageReplacementToken == MessageReplacementToken;
	}
}

/// <summary>
/// Event Handler for Sub events
/// </summary>
public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);

/// <summary>
/// Event Handler for SubItemEndEditing events
/// </summary>
public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditEventArgs e);

/// <summary>
/// Event Args for SubItemClick event
/// </summary>
public class SubItemEventArgs : EventArgs
{
	/// <summary>
	/// 컨트스럭터
	/// </summary>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	public SubItemEventArgs(ListViewItem item, int subItemIndex = -1)
	{
		Item = item;
		SubItemIndex = subItemIndex;
	}

	/// <summary>
	/// 아이템
	/// </summary>
	public ListViewItem Item { get; }

	/// <summary>
	/// 서브 아이템 인덱스
	/// </summary>
	public int SubItemIndex { get; }
}


/// <summary>
/// Event Args for SubItemEndEditingClicked event
/// </summary>
public class SubItemEndEditEventArgs : SubItemEventArgs
{
	/// <summary>
	/// 컨스트럭터
	/// </summary>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	/// <param name="display"></param>
	/// <param name="cancel"></param>
	public SubItemEndEditEventArgs(ListViewItem item, int subItemIndex,
		string? display = null,
		bool cancel = true)
		: base(item, subItemIndex)
	{
		DisplayText = display ?? string.Empty;
		Cancel = cancel;
	}

	/// <summary>
	/// 보여주는 텍스트
	/// </summary>
	public string DisplayText { get; set; }

	/// <summary>
	/// 취소 여부
	/// </summary>
	public bool Cancel { get; set; }
}

/// <summary>
/// 서브 아이템 편집에 사용할 컬렉션
/// </summary>
public enum SubItemEditCollection
{
	/// <summary>
	/// 텍스트 박스
	/// </summary>
	TextBox,
	/// <summary>
	/// 암호로된 텍스트 박스
	/// </summary>
	PasswordTextBox,
	/// <summary>
	/// 날짜시간 선택
	/// </summary>
	DateTime,
	/// <summary>
	/// 컴보박스
	/// </summary>
	ComboBox,
}

#pragma warning restore IDE1006 // 명명 스타일
