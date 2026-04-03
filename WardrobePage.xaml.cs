using Katalog.Models;
using Katalog.Services;
using System.Collections.ObjectModel;

namespace Katalog;

public partial class WardrobePage : ContentPage
{
    private readonly DatabaseService _dbService;
    public ObservableCollection<ClothingItem> Clothes { get; set; } = new();

    public WardrobePage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        ClothesList.ItemsSource = Clothes;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadClothes();
    }

    private async Task LoadClothes()
    {
        var itemsFromDb = await _dbService.GetWardrobeAsync();
        Clothes.Clear();
        foreach (var item in itemsFromDb)
        {
            Clothes.Add(item); 
        }
    }

    private async void OnAddNewClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClothingItem selectedItem)
        {
            ClothesList.SelectedItem = null;
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?ItemId={selectedItem.Id}");
        }
    }
}