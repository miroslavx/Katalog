using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class AddItemPage : ContentPage
{
    private string _imagePath = string.Empty;
    private readonly DatabaseService _dbService;
    private readonly Dictionary<string, List<string>> _categories = new()
{
    { "Ülemine osa", new List<string> { "T-särk", "Särk", "Polo", "Kampsun", "Pusa", "Pintsak", "Pidžaama särk" } },
    { "Alumine osa", new List<string> { "Teksad", "Püksid", "Lühikesed püksid", "Seelik", "Dressipüksid", "Pidžaama püksid" } },
    { "Üleriided", new List<string> { "Jope", "Mantel", "Tagi", "Vest", "Tuulepluus" } },
    { "Jalanõud", new List<string> { "Tossud", "Kingad", "Saapad", "Sandaalid", "Sussid" } },
    { "Aksessuaarid", new List<string> { "Lips", "Müts", "Sall", "Vöö", "Kindad", "Käekell" } }
};

    public AddItemPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;

        CategoryPicker.ItemsSource = _categories.Keys.ToList();
    }

    private void OnCategoryChanged(object sender, EventArgs e)
    {
        var selectedCategory = CategoryPicker.SelectedItem as string;
        if (!string.IsNullOrEmpty(selectedCategory))
        {
            SubCategoryPicker.ItemsSource = _categories[selectedCategory];
            SubCategoryPicker.IsEnabled = true;
        }
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted) status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted) return;

            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                    await sourceStream.CopyToAsync(localFileStream);

                    _imagePath = localFilePath;
                    ItemImage.Source = ImageSource.FromFile(localFilePath);
                }
            }
        }
        catch { await DisplayAlert("Viga", "Kaamerat ei saanud avada.", "OK"); }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_imagePath) || CategoryPicker.SelectedItem == null || SubCategoryPicker.SelectedItem == null)
        {
            await DisplayAlert("Oota", "Palun tee pilt ja vali kategooriad!", "OK");
            return;
        }

        var newItem = new ClothingItem
        {
            Name = string.IsNullOrWhiteSpace(NameEntry.Text) ? "Nimetu" : NameEntry.Text,
            Category = CategoryPicker.SelectedItem.ToString(),
            SubCategory = SubCategoryPicker.SelectedItem.ToString(),
            ImagePath = _imagePath
        };

        await _dbService.AddItemAsync(newItem);
        await Shell.Current.GoToAsync("..");
    }
}