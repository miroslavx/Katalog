using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class StylePage : ContentPage
{
    private readonly StylistService _stylistService;

    public StylePage(StylistService stylistService)
    {
        InitializeComponent();
        _stylistService = stylistService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Generate("Ametlik");
    }

    private void OnStyleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            string style = btn.CommandParameter?.ToString() ?? "Ametlik";
            Generate(style);

            if (btn.Parent is HorizontalStackLayout parent)
            {
                foreach (var child in parent.Children)
                {
                    if (child is Button otherBtn)
                    {
                        otherBtn.BackgroundColor = Color.FromArgb("#E5E5EA");
                        otherBtn.TextColor = Color.FromArgb("#000000");
                    }
                }
            }
            btn.BackgroundColor = Color.FromArgb("#007AFF");
            btn.TextColor = Color.FromArgb("#FFFFFF");
        }
    }

    private async void Generate(string style)
    {
        BindableLayout.SetItemsSource(OutfitsList, null);

        var result = await _stylistService.GenerateOutfitsByStyleAsync(style);

        if (result.Success && result.Outfits != null && result.Outfits.Any())
        {
            BindableLayout.SetItemsSource(OutfitsList, result.Outfits);
        }
        else
        {
            await DisplayAlert("Info", result.Message, "OK");
        }
    }
}