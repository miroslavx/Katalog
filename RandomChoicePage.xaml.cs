using Katalog.Models;
using Katalog.Services;
using System.Collections.ObjectModel;

namespace Katalog;

public partial class RandomChoicePage : ContentPage
{
    private readonly DatabaseService _dbService;
    public ObservableCollection<ClothingItem> Clothes { get; set; } = new();

    private ClothingItem _item1;
    private ClothingItem _item2;

    public RandomChoicePage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        ClothesList.ItemsSource = Clothes;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var items = await _dbService.GetWardrobeAsync();
        Clothes.Clear();
        foreach (var item in items) Clothes.Add(item);
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClothingItem selectedItem)
        {
            ClothesList.SelectedItem = null; 

            if (_item1 == null)
            {
                _item1 = selectedItem;
                Item1Image.Source = ImageSource.FromFile(_item1.ImagePath);
                Item1Image.IsVisible = true;
            }
            else if (_item2 == null && selectedItem.Id != _item1.Id)
            {
                _item2 = selectedItem;
                Item2Image.Source = ImageSource.FromFile(_item2.ImagePath);
                Item2Image.IsVisible = true;

                SpinButton.IsEnabled = true;
            }
            else
            {
                ResetArena();
                _item1 = selectedItem;
                Item1Image.Source = ImageSource.FromFile(_item1.ImagePath);
                Item1Image.IsVisible = true;
            }
        }
    }

    private void ResetArena()
    {
        _item1 = null;
        _item2 = null;
        Item1Image.IsVisible = false;
        Item2Image.IsVisible = false;
        SpinButton.IsEnabled = false;
        Item1Border.Stroke = Colors.Transparent;
        Item2Border.Stroke = Colors.Transparent;
        Item1Border.Opacity = 1;
        Item2Border.Opacity = 1;
        SpinningArrow.Rotation = 0;
    }

    private async void OnSpinClicked(object sender, EventArgs e)
    {
        if (_item1 == null || _item2 == null) return;

        SpinButton.IsEnabled = false;
        ClothesList.IsEnabled = false;
        Item1Border.Stroke = Colors.Transparent;
        Item2Border.Stroke = Colors.Transparent;
        Item1Border.Opacity = 1;
        Item2Border.Opacity = 1;
        SpinningArrow.Rotation = 0;
        bool item1Wins = new Random().Next(2) == 0;

        double targetAngle = item1Wins ? (1800 - 90) : (1800 + 90);
        await SpinningArrow.RotateTo(targetAngle, 3000, Easing.CubicOut);
        if (item1Wins)
        {
            Item1Border.Stroke = Color.FromArgb("#007AFF"); 
            await Item2Border.FadeTo(0.3, 500); 
        }
        else
        {
            Item2Border.Stroke = Color.FromArgb("#007AFF");
            await Item1Border.FadeTo(0.3, 500);
        }
        string winnerName = item1Wins ? _item1.Name : _item2.Name;
        await DisplayAlert("Tulemus", $"Täna sa kannad: {winnerName}!", "Vinge!");

        SpinButton.IsEnabled = true;
        ClothesList.IsEnabled = true;
    }
}