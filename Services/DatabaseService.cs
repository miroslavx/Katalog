using SQLite;
using Katalog.Models;

namespace Katalog.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;

        public async Task InitAsync()
        {
            if (_db != null) return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "WardrobeData.db3");
            _db = new SQLiteAsyncConnection(databasePath);
            await _db.CreateTableAsync<ClothingItem>();
        }

        public async Task<int> AddItemAsync(ClothingItem item)
        {
            await InitAsync();
            return await _db.InsertAsync(item);
        }

        public async Task<List<ClothingItem>> GetWardrobeAsync()
        {
            await InitAsync();
            return await _db.Table<ClothingItem>().ToListAsync();
        }
    }
}