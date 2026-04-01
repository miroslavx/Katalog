using SmartWardrobe.Models;

namespace SmartWardrobe.Services
{
    public class StylistService
    {
        private readonly DatabaseService _dbService;
        private Random _random = new Random();

        public StylistService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // 1. Случайный выбор ("Монетка")
        public async Task<List<ClothingItem>> GetRandomOutfitAsync()
        {
            var allItems = await _dbService.GetWardrobeAsync();

            var top = allItems.Where(c => c.Category == "Top").OrderBy(x => _random.Next()).FirstOrDefault();
            var bottom = allItems.Where(c => c.Category == "Bottom").OrderBy(x => _random.Next()).FirstOrDefault();
            var shoes = allItems.Where(c => c.Category == "Shoes").OrderBy(x => _random.Next()).FirstOrDefault();

            return new List<ClothingItem> { top, bottom, shoes };
        }

        // 2. Выбор по пресету (например "Официальный")
        public async Task<List<ClothingItem>> GetOutfitByPresetAsync(string preset)
        {
            var items = await _dbService.GetWardrobeAsync();

            var top = items.Where(c => c.Category == "Top" && c.PresetType == preset).FirstOrDefault();
            var bottom = items.Where(c => c.Category == "Bottom" && c.PresetType == preset).FirstOrDefault();

            return new List<ClothingItem> { top, bottom };
        }

        // 3. Выбор по базовой температуре (пока без интернета, ручной ввод для теста)
        public async Task<List<ClothingItem>> GetOutfitByTemperatureAsync(int currentTemp)
        {
            var items = await _dbService.GetWardrobeAsync();

            var top = items.Where(c => c.Category == "Top" && currentTemp >= c.MinTemp && currentTemp <= c.MaxTemp).FirstOrDefault();
            var bottom = items.Where(c => c.Category == "Bottom" && currentTemp >= c.MinTemp && currentTemp <= c.MaxTemp).FirstOrDefault();

            return new List<ClothingItem> { top, bottom };
        }
    }
}