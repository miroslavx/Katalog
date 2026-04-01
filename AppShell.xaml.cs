namespace Katalog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(WardrobePage), typeof(WardrobePage));
        Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
    }
}