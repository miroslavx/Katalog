using Katalog.Models;
using Katalog.Services;
using System.Collections.ObjectModel;

namespace Katalog;

public partial class StylePage : ContentPage
{
    private readonly StylistService _stylistService;
    public ObservableCollection<OutfitSet> Outfits { get; set; } = new();

    public StylePage(StylistService stylistService)
    {
        InitializeComponent();
        _stylistService = stylistService;
        OutfitsList.ItemsSource = Outfits;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // При открытии страницы сразу генерируем "Ametlik" (Официальный)
        Generate("Ametlik");
    }

    private void OnStyleClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            // Получаем название стиля из CommandParameter кнопки
            string style = btn.CommandParameter.ToString();
            Generate(style);

            // Визуально делаем нажатую кнопку синей, остальные - серыми
            var parent = btn.Parent as HorizontalStackLayout;
            foreach (var child in parent.Children)
            {
                if (child is Button otherBtn)
                {
                    otherBtn.BackgroundColor = Color.FromArgb("#E5E5EA");
                    otherBtn.TextColor = Color.FromArgb("#000000");
                }
            }
            btn.BackgroundColor = Color.FromArgb("#007AFF");
            btn.TextColor = Color.FromArgb("#FFFFFF");
        }
    }

    private async void Generate(string style)
    {
        Outfits.Clear();
        var result = await _stylistService.GenerateOutfitsByStyleAsync(style);

        if (result.Success)
        {
            foreach (var outfit in result.Outfits)
            {
                Outfits.Add(outfit);
            }
        }
        else
        {
            // Если вещей нет - выводим ошибку алгоритма
            await DisplayAlert("Vabandust!", result.Message, "Selge");
        }
    }
}