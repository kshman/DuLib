namespace Du.Blazor.Components;

public partial class Badge
{
    public record Settings
    {
        public ComponentColor Fore { get; set; }
        public ComponentColor Back { get; set; }
        public BadgeLayout Layout { get; set; }
        public string? Css { get; set; }
    }

    public static Settings DefaultSettings { get; set; } = new()
    {
        Fore = ComponentColor.Light,
        Back = ComponentColor.Primary,
		Layout = BadgeLayout.Standard,
        Css = null,
    };
}
