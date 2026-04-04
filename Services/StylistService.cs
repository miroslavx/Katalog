using Katalog.Models;

namespace Katalog.Services
{
    public class StylistService
    {
        private readonly DatabaseService _dbService;
        private Random _random = new Random();

        public StylistService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<List<ClothingItem>> GetRandomOutfitAsync()
        {
            var allItems = await _dbService.GetWardrobeAsync();

            var top = allItems.Where(c => c.Category == "Top").OrderBy(x => _random.Next()).FirstOrDefault();
            var bottom = allItems.Where(c => c.Category == "Bottom").OrderBy(x => _random.Next()).FirstOrDefault();
            var shoes = allItems.Where(c => c.Category == "Shoes").OrderBy(x => _random.Next()).FirstOrDefault();

            return new List<ClothingItem> { top, bottom, shoes };
        }

        public async Task<List<ClothingItem>> GetOutfitByPresetAsync(string preset)
        {
            var items = await _dbService.GetWardrobeAsync();

            var top = items.Where(c => c.Category == "Top" && c.PresetType == preset).FirstOrDefault();
            var bottom = items.Where(c => c.Category == "Bottom" && c.PresetType == preset).FirstOrDefault();

            return new List<ClothingItem> { top, bottom };
        }

        public async Task<List<ClothingItem>> GetOutfitByTemperatureAsync(int currentTemp)
        {
            var items = await _dbService.GetWardrobeAsync();

            var top = items.Where(c => c.Category == "Top" && currentTemp >= c.MinTemp && currentTemp <= c.MaxTemp).FirstOrDefault();
            var bottom = items.Where(c => c.Category == "Bottom" && currentTemp >= c.MinTemp && currentTemp <= c.MaxTemp).FirstOrDefault();


            return new List<ClothingItem> { top, bottom };

        }
        public async Task<(bool Success, string Message, List<OutfitSet> Outfits)> GenerateOutfitsByStyleAsync(string style)
        {
            var wardrobe = await _dbService.GetWardrobeAsync();

            // Правила стилей (какие подкатегории допускаются)
            List<string> allowedTops = new();
            List<string> allowedBottoms = new();
            List<string> allowedShoes = new();
            List<string> allowedAccessories = new();

            switch (style)
            {
                case "Ametlik": // Официальный
                    allowedTops = new List<string> { "Särk", "Pintsak" };
                    allowedBottoms = new List<string> { "Püksid", "Seelik" };
                    allowedShoes = new List<string> { "Kingad" };
                    allowedAccessories = new List<string> { "Lips", "Vöö", "Käekell" };
                    break;
                case "Sportlik": // Спортивный
                    allowedTops = new List<string> { "T-särk", "Polo" };
                    allowedBottoms = new List<string> { "Lühikesed püksid", "Dressipüksid" };
                    allowedShoes = new List<string> { "Tossud" };
                    allowedAccessories = new List<string> { "Müts" };
                    break;
                case "Pidu": // Вечеринка
                    allowedTops = new List<string> { "Särk", "Polo", "Pusa" };
                    allowedBottoms = new List<string> { "Teksad", "Püksid", "Seelik" };
                    allowedShoes = new List<string> { "Kingad", "Tossud" };
                    break;
                case "Magamiseks": // Ночной (Пижамы)
                    allowedTops = new List<string> { "T-särk", "Pidžaama särk" };
                    allowedBottoms = new List<string> { "Lühikesed püksid", "Pidžaama püksid" };
                    allowedShoes = new List<string> { "Sussid" };
                    break;
                case "Igapäevane": // Кэжуал
                    allowedTops = new List<string> { "T-särk", "Kampsun", "Pusa" };
                    allowedBottoms = new List<string> { "Teksad" };
                    allowedShoes = new List<string> { "Tossud", "Saapad" };
                    break;
            }

            // Фильтруем шкаф по правилам
            var tops = wardrobe.Where(x => allowedTops.Contains(x.SubCategory)).ToList();
            var bottoms = wardrobe.Where(x => allowedBottoms.Contains(x.SubCategory)).ToList();
            var shoes = wardrobe.Where(x => allowedShoes.Contains(x.SubCategory)).ToList();
            var accs = wardrobe.Where(x => allowedAccessories.Contains(x.SubCategory)).ToList();

            // ПРОВЕРКА: Если нет хотя бы верха или низа - лук не собрать!
            if (!tops.Any() || !bottoms.Any())
            {
                return (false, $"Sul puuduvad vajalikud riided ({style} stiil). Lisa kappi sobiv ülemine või alumine osa!", null);
            }
            var outfits = new List<OutfitSet>();
            var rand = new Random();

            // Перемешиваем вещи для разнообразия
            tops = tops.OrderBy(x => rand.Next()).ToList();
            bottoms = bottoms.OrderBy(x => rand.Next()).ToList();

            int variations = Math.Min(3, Math.Max(tops.Count, bottoms.Count));

            for (int i = 0; i < variations; i++)
            {
                var currentSet = new List<ClothingItem>
        {
            tops[i % tops.Count],       // Обязательно: Верх
            bottoms[i % bottoms.Count]  // Обязательно: Низ
        };

                // Необязательно: Обувь (если есть)
                if (shoes.Any()) currentSet.Add(shoes[rand.Next(shoes.Count)]);

                // Необязательно: Аксессуар (с вероятностью 50%, чтобы не всегда лепить галстук)
                if (accs.Any() && rand.Next(2) == 0) currentSet.Add(accs[rand.Next(accs.Count)]);

                outfits.Add(new OutfitSet { Title = $"Stiilne valik {i + 1}", Items = currentSet });
            }

            return (true, "Valmis!", outfits);
        }
    }
}