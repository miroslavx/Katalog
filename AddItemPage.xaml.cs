using Katalog.Models;
using Katalog.Services;
using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class AddItemPage : ContentPage
{
    private string _imagePath;
    private readonly DatabaseService _dbService;
    public AddItemPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            // 1. ПРОВЕРЯЕМ И ЗАПРАШИВАЕМ РАЗРЕШЕНИЕ НА КАМЕРУ
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            // Если юзер нажал "Запретить"
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Viga", "Kaamera luba on vajalik pildi tegemiseks!", "OK");
                return;
            }

            // 2. ЕСЛИ РАЗРЕШИЛ - ОТКРЫВАЕМ КАМЕРУ
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                    // Копируем файл из камеры в нашу папку
                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                    await sourceStream.CopyToAsync(localFileStream);

                    // Запоминаем путь для базы данных
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
            await DisplayAlert("Viga", $"Midagi läks valesti: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_imagePath))
        {
            await DisplayAlert("Oota", "Palun tee enne pilti!", "OK");
            return;
        }

        var newItem = new ClothingItem
        {
            Name = NameEntry.Text ?? "Nimetu",
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Muu",
            ImagePath = _imagePath 
        };
        await _dbService.AddItemAsync(newItem);
        await Shell.Current.GoToAsync("..");
    }
}