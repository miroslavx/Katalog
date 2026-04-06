using Katalog.Models;
using Katalog.Services;

namespace Katalog;

[QueryProperty(nameof(ItemId), "ItemId")]
public partial class ItemDetailPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private ClothingItem _currentItem;

    public string ItemId
    {
        set { LoadItem(int.Parse(value)); }
    }

    public ItemDetailPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void LoadItem(int id)
    {
        _currentItem = await _dbService.GetItemAsync(id);
        if (_currentItem != null)
        {
            ItemImage.Source = ImageSource.FromFile(_currentItem.ImagePath);
            NameEntry.Text = _currentItem.Name;

            CategoryPicker.ItemsSource = CategoryData.Categories.Keys.ToList();
            CategoryPicker.SelectedItem = _currentItem.Category;

            if (CategoryData.Categories.ContainsKey(_currentItem.Category))
            {
                SubCategoryPicker.ItemsSource = CategoryData.Categories[_currentItem.Category];
                SubCategoryPicker.SelectedItem = _currentItem.SubCategory;
            }
        }
    }

    private void OnCategoryChanged(object sender, EventArgs e)
    {
        var selectedCategory = CategoryPicker.SelectedItem as string;
        if (!string.IsNullOrEmpty(selectedCategory) && CategoryData.Categories.ContainsKey(selectedCategory))
        {
            SubCategoryPicker.ItemsSource = CategoryData.Categories[selectedCategory];
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        _currentItem.Name = NameEntry.Text;
        _currentItem.Category = CategoryPicker.SelectedItem?.ToString() ?? _currentItem.Category;
        _currentItem.SubCategory = SubCategoryPicker.SelectedItem?.ToString() ?? _currentItem.SubCategory;

        await _dbService.UpdateItemAsync(_currentItem);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Kustuta", "Oled kindel, et soovid selle eseme kustutada?", "Jah", "Ei");
        if (answer)
        {
            await _dbService.DeleteItemAsync(_currentItem);
            await Shell.Current.GoToAsync("..");
        }
    }
}