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

    // Юзер кликает по вещам, чтобы заполнить слоты
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ClothingItem selectedItem)
        {
            ClothesList.SelectedItem = null; // Сбрасываем клик

            if (_item1 == null)
            {
                // Заполняем слот 1
                _item1 = selectedItem;
                Item1Image.Source = ImageSource.FromFile(_item1.ImagePath);
                Item1Image.IsVisible = true;
            }
            else if (_item2 == null && selectedItem.Id != _item1.Id)
            {
                // Заполняем слот 2
                _item2 = selectedItem;
                Item2Image.Source = ImageSource.FromFile(_item2.ImagePath);
                Item2Image.IsVisible = true;

                // Включаем синюю кнопку
                SpinButton.IsEnabled = true;
            }
            else
            {
                // Если оба слота заняты, а юзер жмет снова - сбрасываем и начинаем заново
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

    // Анимация рулетки!
    private async void OnSpinClicked(object sender, EventArgs e)
    {
        if (_item1 == null || _item2 == null) return;

        // Блокируем интерфейс на время анимации
        SpinButton.IsEnabled = false;
        ClothesList.IsEnabled = false;

        // Сбрасываем стили (если юзер крутит второй раз)
        Item1Border.Stroke = Colors.Transparent;
        Item2Border.Stroke = Colors.Transparent;
        Item1Border.Opacity = 1;
        Item2Border.Opacity = 1;
        SpinningArrow.Rotation = 0;

        // 50/50 - кто победит? (0 = левый, 1 = правый)
        bool item1Wins = new Random().Next(2) == 0;

        // Высчитываем угол: 5 полных оборотов (1800 град.) + поворот налево (-90) или направо (+90)
        double targetAngle = item1Wins ? (1800 - 90) : (1800 + 90);

        // КРУТИМ СТРЕЛКУ! (3 секунды, плавная остановка)
        await SpinningArrow.RotateTo(targetAngle, 3000, Easing.CubicOut);

        // Подсвечиваем победителя и затемняем проигравшего
        if (item1Wins)
        {
            Item1Border.Stroke = Color.FromArgb("#007AFF"); // Синяя рамка
            await Item2Border.FadeTo(0.3, 500);             // Затухание лузера
        }
        else
        {
            Item2Border.Stroke = Color.FromArgb("#007AFF");
            await Item1Border.FadeTo(0.3, 500);
        }

        // Выводим попап
        string winnerName = item1Wins ? _item1.Name : _item2.Name;
        await DisplayAlert("Tulemus", $"Täna sa kannad: {winnerName}!", "Vinge!");

        SpinButton.IsEnabled = true;
        ClothesList.IsEnabled = true;
    }
}