using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class WeatherOutfitPage : ContentPage
{
    private readonly WeatherService _weatherService;
    private readonly StylistService _stylistService;

    public WeatherOutfitPage(WeatherService weatherService, StylistService stylistService)
    {
        InitializeComponent();
        _weatherService = weatherService;
        _stylistService = stylistService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadWeatherAndOutfit();
    }

    private async Task LoadWeatherAndOutfit()
    {
        LoadingSpinner.IsRunning = true;
        LoadingSpinner.IsVisible = true;
        OutfitContainer.IsVisible = false;

        // 1. Получаем погоду
        var weather = await _weatherService.GetWeatherAsync();

        if (weather == null)
        {
            await DisplayAlert("Viga", "Ei saanud ilmainfot. Kontrolli internetti või API võtit.", "OK");
            LoadingSpinner.IsRunning = false;
            LoadingSpinner.IsVisible = false;
            return;
        }

        // 2. Обновляем UI погоды
        CityLabel.Text = weather.Name;
        TempLabel.Text = $"{Math.Round(weather.Main.Temp)}°C";
        DescLabel.Text = weather.Weather.FirstOrDefault()?.Main ?? "Teadmata";

        var result = await _stylistService.GenerateOutfitByWeatherAsync(weather);

        if (result.Success && result.Outfit != null)
        {
            BindableLayout.SetItemsSource(OutfitList, result.Outfit.Items);

            OutfitContainer.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Vabandust", result.Message, "OK");
        }

        LoadingSpinner.IsRunning = false;
        LoadingSpinner.IsVisible = false;
    }
}