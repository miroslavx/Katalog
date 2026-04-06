using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class WardrobePage : ContentPage
{
    private readonly DatabaseService _dbService;
    private List<ClothingItem> _allItems = new();

    public WardrobePage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadClothes();
    }

    private async Task LoadClothes()
    {
        _allItems = await _dbService.GetWardrobeAsync();

        FilterPicker.SelectedIndexChanged -= OnFilterSortChanged;
        SortPicker.SelectedIndexChanged -= OnFilterSortChanged;

        if (FilterPicker.SelectedIndex == -1) FilterPicker.SelectedIndex = 0;
        if (SortPicker.SelectedIndex == -1) SortPicker.SelectedIndex = 0;

        FilterPicker.SelectedIndexChanged += OnFilterSortChanged;
        SortPicker.SelectedIndexChanged += OnFilterSortChanged;

        ApplyFilterAndSort();
    }

    private void OnFilterSortChanged(object sender, EventArgs e)
    {
        ApplyFilterAndSort();
    }

    private void ApplyFilterAndSort()
    {
        if (_allItems == null || !_allItems.Any())
        {
            ClothesList.ItemsSource = new List<ClothingItem>();
            return;
        }

        var filtered = _allItems.AsEnumerable();

        string filter = FilterPicker.SelectedItem?.ToString() ?? "Kõik riided";
        if (filter != "Kõik riided")
        {
            filtered = filtered.Where(x => x.Category == filter);
        }

        string sort = SortPicker.SelectedItem?.ToString() ?? "Uusimad enne";
        if (sort == "Uusimad enne") filtered = filtered.OrderByDescending(x => x.Id);
        else if (sort == "Vanemad enne") filtered = filtered.OrderBy(x => x.Id);
        else if (sort == "A-Z (Tüübi järgi)") filtered = filtered.OrderBy(x => x.SubCategory);
        else if (sort == "Z-A (Tüübi järgi)") filtered = filtered.OrderByDescending(x => x.SubCategory);

        ClothesList.ItemsSource = filtered.ToList();
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