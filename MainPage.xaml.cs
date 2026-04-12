namespace Katalog;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RandomChoicePage));
    }

    private async void OnPresetClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StylePage));
    }

    private async void OnOpenWardrobeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WardrobePage));
    }

    private async void OnWeatherClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WeatherOutfitPage));
    }
}