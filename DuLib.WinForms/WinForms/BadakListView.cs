namespace Du.WinForms;

/// <summary>
/// 바닥 리스느뷰
/// </summary>
public class BadakListView : ListView
{
	private const string c_reorder_mesg = "(줄 위치 바꾸기)";

	private bool _item_reorder;

	/// <summary>
	/// 아이템 순서를 바꿨을때
	/// </summary>
	[Description("아이템 순서를 바꾸면 호출합니다")]
	public ItemReorderDragHandler? ItemReordered;

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
	/// 컨스트럭터
	/// </summary>
	public BadakListView()
	{ }

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
