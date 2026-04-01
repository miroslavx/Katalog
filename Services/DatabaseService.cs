using SQLite;
using SmartWardrobe.Models;

namespace SmartWardrobe.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        // Инициализация базы данных
        public async Task InitAsync()
        {
            if (_db != null) return;

            // База будет храниться в скрытой папке телефона
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WardrobeData.db3");
            _db = new SQLiteAsyncConnection(databasePath);

            // Создаем таблицу
            await _db.CreateTableAsync<ClothingItem>();
        }

        // Добавить вещь
        public async Task<int> AddItemAsync(ClothingItem item)
        {
            await InitAsync();
            return await _db.InsertAsync(item);
        }

        // Получить весь шкаф
        public async Task<List<ClothingItem>> GetWardrobeAsync()
        {
            await InitAsync();
            return await _db.Table<ClothingItem>().ToListAsync();
        }
    }
}