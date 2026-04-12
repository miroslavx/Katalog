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
        public async Task<int> UpdateItemAsync(ClothingItem item)
        {
            await InitAsync();
            return await _db.UpdateAsync(item);
        }
        public async Task<int> DeleteItemAsync(ClothingItem item)
        {
            await InitAsync();
            return await _db.DeleteAsync(item);
        }

        public async Task<ClothingItem> GetItemAsync(int id)
        {
            await InitAsync();
            return await _db.Table<ClothingItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }
    }
}