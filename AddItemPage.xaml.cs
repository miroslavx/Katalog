using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class AddItemPage : ContentPage
{
    private string _imagePath = string.Empty;
    private readonly DatabaseService _dbService;

    public AddItemPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        CategoryPicker.ItemsSource = CategoryData.Categories.Keys.ToList();
    }

    private void OnCategoryChanged(object sender, EventArgs e)
    {
        var selectedCategory = CategoryPicker.SelectedItem as string;
        if (!string.IsNullOrEmpty(selectedCategory) && CategoryData.Categories.ContainsKey(selectedCategory))
        {
            SubCategoryPicker.ItemsSource = CategoryData.Categories[selectedCategory];
            SubCategoryPicker.IsEnabled = true;
        }
    }

    private void OnShowInstructionClicked(object sender, EventArgs e)
    {
        InstructionOverlay.IsVisible = true;
    }

    private void OnCancelInstructionClicked(object sender, EventArgs e)
    {
        InstructionOverlay.IsVisible = false;
    }

    private async void OnProceedToCameraClicked(object sender, EventArgs e)
    {
        InstructionOverlay.IsVisible = false;

        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Viga", "Kaamera luba on vajalik pildi tegemiseks!", "OK");
                return;
            }

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
            else
            {
                await DisplayAlert("Viga", "Sinu seade ei toeta kaamerat.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Viga", $"Kaamerat ei saanud avada: {ex.Message}", "OK");
        }
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
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Muu",
            SubCategory = SubCategoryPicker.SelectedItem?.ToString() ?? "Muu",
            ImagePath = _imagePath
        };

        await _dbService.AddItemAsync(newItem);
        await Shell.Current.GoToAsync("..");
    }
}