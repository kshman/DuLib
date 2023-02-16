namespace Du.Blazor.Components;

public class DuCarousel : DuComponentParent, IAsyncDisposable
{
    [CascadingParameter] public DuGroupCarousel? Group { get; set; }

    protected override string RootClass => "carousel-item";

    //
    protected override async Task OnInitializedAsync()
    {
        if (Group != null)
            await Group.AddItemAsync(this);
    }

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
