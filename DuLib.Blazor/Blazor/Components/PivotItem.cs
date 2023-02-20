namespace Du.Blazor.Components;

public class PivotItem : ComponentContainer, IAsyncDisposable
{
	/// <summary>피벗 그룹</summary>
	[CascadingParameter] public Pivot? Group { get; set; }

	/// <summary>타이틀 <see cref="Display"/> </summary>
	[Parameter] public string? Title { get; set; }

	/// <summary>헤더 <see cref="Title"/></summary>
	[Parameter] public RenderFragment? Display { get; set; }
	/// <summary>내용
	/// <see cref="Display"/>와 짝꿍<br/>이 내용이 있을 경우,
	/// 태그 밖 <see cref="ComponentContainer.ChildContent"/>는 처리하지 않는다
	/// </summary>
	[Parameter] public RenderFragment? Content { get; set; }

	//
	protected override string CssName => "hpvt-item";

	//
	internal bool Selected { get; set; }

	//
	protected override async Task OnInitializedAsync()
	{
		if (Group != null)
			await Group.AddItemAsync(this);
	}

	//
	protected override void OnComponentClass(CssCompose css) => 
		css.Register(() => Selected.IfTrue("hpvt-sel"));

	//
	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	//
	protected virtual async ValueTask DisposeAsyncCore()
	{
		if (Group != null)
			await Group.RemoveItemAsync(this);
	}
}
