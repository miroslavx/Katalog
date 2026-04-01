using Katalog.Models;
using Katalog.Services;
using System.Collections.ObjectModel;

namespace Katalog;

public partial class WardrobePage : ContentPage
{
    private readonly DatabaseService _dbService;

    // Эта коллекция автоматически обновляет экран, когда в ней меняются данные
    public ObservableCollection<ClothingItem> Clothes { get; set; } = new();

    public WardrobePage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;

        // Говорим интерфейсу брать данные из этого файла
        ClothesList.ItemsSource = Clothes;
    }

    // Этот метод срабатывает каждый раз, когда мы открываем эту страницу
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
            Clothes.Add(item); // Добавляем на экран вещи из базы
        }
    }

    private async void OnAddNewClicked(object sender, EventArgs e)
    {
        // Переходим на страницу добавления вещи (Камеры)
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }
}