namespace Katalog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(WardrobePage), typeof(WardrobePage));
        Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
        Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
        Routing.RegisterRoute(nameof(RandomChoicePage), typeof(RandomChoicePage));
        Routing.RegisterRoute(nameof(StylePage), typeof(StylePage));
    }
}