namespace Katalog;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnRandomClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tulemus", "Siin on sinu juhuslik riietus!", "OK");
    }

    private async void OnPresetClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tulemus", "Ametlik riietus leitud.", "OK");
    }

    private async void OnWeatherClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Tulemus", "Õues on +15°C. Pakun kerget jopet.", "OK");
    }

    private async void OnOpenWardrobeClicked(object sender, EventArgs e)
    {
        // Переход на страницу со списком одежды
        // await Navigation.PushAsync(new WardrobePage());
    }
}