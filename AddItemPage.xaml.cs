using Katalog.Models;
using Katalog.Services;
using Katalog.Models;
using Katalog.Services;

namespace Katalog;

public partial class AddItemPage : ContentPage
{
    private string _imagePath;
    private readonly DatabaseService _dbService;

    // Внедряем базу данных через конструктор
    public AddItemPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            // Открываем камеру телефона (или эмулятора)
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // Создаем путь для сохранения фото в скрытой папке телефона
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                // Копируем файл из камеры в нашу папку
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);

                // Запоминаем путь для базы данных
                _imagePath = localFilePath;

                // Показываем фото на экране
                ItemImage.Source = ImageSource.FromFile(localFilePath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Viga", "Kaamerat ei saanud avada.", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_imagePath))
        {
            await DisplayAlert("Oota", "Palun tee enne pilti!", "OK");
            return;
        }

        // Создаем объект вещи
        var newItem = new ClothingItem
        {
            Name = NameEntry.Text ?? "Nimetu",
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Muu",
            ImagePath = _imagePath // Тот самый путь к фото на телефоне
        };

        // Сохраняем в SQLite
        await _dbService.AddItemAsync(newItem);

        // Возвращаемся на предыдущий экран
        await Shell.Current.GoToAsync("..");
    }
}