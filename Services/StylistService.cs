using Katalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Katalog.Services
{
    public class StylistService
    {
        private readonly DatabaseService _dbService;

        public StylistService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // =========================================================
        // 1. АЛГОРИТМ: ГЕНЕРАЦИЯ ОДЕЖДЫ ПО СТИЛЮ (ДЛЯ ПРЕСЕТОВ)
        // =========================================================
        public async Task<(bool Success, string Message, List<OutfitSet>? Outfits)> GenerateOutfitsByStyleAsync(string style)
        {
            var wardrobe = await _dbService.GetWardrobeAsync();

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

            // Собираем до 3 вариантов
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

                // Необязательно: Обувь (если есть, добавляем случайную)
                if (shoes.Any()) currentSet.Add(shoes[rand.Next(shoes.Count)]);

                // Необязательно: Аксессуар (с вероятностью 50%, чтобы не перегружать лук)
                if (accs.Any() && rand.Next(2) == 0) currentSet.Add(accs[rand.Next(accs.Count)]);

                outfits.Add(new OutfitSet { Title = $"Stiilne valik {i + 1}", Items = currentSet });
            }

            return (true, "Valmis!", outfits);
        }

        // =========================================================
        // 2. АЛГОРИТМ: ГЕНЕРАЦИЯ ОДЕЖДЫ ПО ПОГОДЕ (OPENWEATHERMAP)
        // =========================================================
        public async Task<(bool Success, string Message, OutfitSet? Outfit)> GenerateOutfitByWeatherAsync(WeatherResponse weather)
        {
            var wardrobe = await _dbService.GetWardrobeAsync();
            var rand = new Random();

            double temp = weather.Main.Temp;

            // Проверяем, есть ли дождь в описании погоды
            bool isRaining = weather.Weather.Any(w => w.Main.Contains("Rain", StringComparison.OrdinalIgnoreCase) || w.Main.Contains("Drizzle", StringComparison.OrdinalIgnoreCase));

            List<string> allowedTops = new();
            List<string> allowedBottoms = new();
            List<string> allowedOuterwear = new();
            List<string> allowedShoes = new();

            // ПРАВИЛО 1: ЛЕТО (Жара >= 20°C)
            if (temp >= 20)
            {
                allowedTops.AddRange(new[] { "T-särk", "Polo", "Särk" });
                allowedBottoms.AddRange(new[] { "Lühikesed püksid", "Seelik" });
                allowedShoes.AddRange(new[] { "Sandaalid", "Tossud" });
            }
            // ПРАВИЛО 2: ТЕПЛАЯ ОСЕНЬ / ВЕСНА (10°C - 19°C)
            else if (temp >= 10 && temp < 20)
            {
                allowedTops.AddRange(new[] { "Särk", "T-särk" });
                allowedBottoms.AddRange(new[] { "Teksad", "Püksid", "Seelik" });
                allowedOuterwear.AddRange(new[] { "Tagi", "Tuulepluus", "Pusa", "Kampsun" });
                allowedShoes.AddRange(new[] { "Tossud", "Kingad" });
            }
            // ПРАВИЛО 3: ХОЛОДНАЯ ОСЕНЬ / МЯГКАЯ ЗИМА (0°C - 9°C)
            else if (temp >= 0 && temp < 10)
            {
                allowedTops.AddRange(new[] { "Pusa", "Kampsun", "Särk" });
                allowedBottoms.AddRange(new[] { "Teksad", "Püksid" });
                allowedOuterwear.AddRange(new[] { "Jope", "Mantel" });
                allowedShoes.AddRange(new[] { "Saapad", "Tossud" });
            }
            // ПРАВИЛО 4: СУРОВАЯ ЗИМА (< 0°C)
            else
            {
                allowedTops.AddRange(new[] { "Kampsun", "Pusa" });
                allowedBottoms.AddRange(new[] { "Teksad", "Dressipüksid" });
                allowedOuterwear.AddRange(new[] { "Jope" }); // Строго зимняя куртка
                allowedShoes.AddRange(new[] { "Saapad" });
            }

            // ПРАВИЛО 5: ПРОВЕРКА НА ДОЖДЬ (Переопределяет некоторые вещи)
            if (isRaining)
            {
                // Обязательно куртка от дождя (заменяем свитера на куртки)
                allowedOuterwear.Clear();
                allowedOuterwear.AddRange(new[] { "Tuulepluus", "Jope", "Mantel", "Tagi" });

                // Никаких сандалий
                allowedShoes.Remove("Sandaalid");
                if (!allowedShoes.Any()) allowedShoes.Add("Saapad"); // Запасной вариант
            }

            // Фильтруем вещи пользователя
            var tops = wardrobe.Where(x => allowedTops.Contains(x.SubCategory)).ToList();
            var bottoms = wardrobe.Where(x => allowedBottoms.Contains(x.SubCategory)).ToList();
            var outerwears = wardrobe.Where(x => allowedOuterwear.Contains(x.SubCategory)).ToList();
            var shoes = wardrobe.Where(x => allowedShoes.Contains(x.SubCategory)).ToList();

            // Если не хватает базы (верха или низа) - прерываем
            if (!tops.Any() || !bottoms.Any())
            {
                return (false, "Sul pole selle ilma jaoks sobivaid baasriideid (ülemine või alumine osa).", null);
            }

            // Собираем идеальный комплект
            var finalItems = new List<ClothingItem>
            {
                tops[rand.Next(tops.Count)],
                bottoms[rand.Next(bottoms.Count)]
            };

            // Добавляем верхнюю одежду (если холодно или идет дождь)
            if (outerwears.Any() && (temp < 20 || isRaining))
            {
                finalItems.Add(outerwears[rand.Next(outerwears.Count)]);
            }

            // Добавляем обувь
            if (shoes.Any())
            {
                finalItems.Add(shoes[rand.Next(shoes.Count)]);
            }

            // Формируем результат с красивым заголовком
            var outfit = new OutfitSet
            {
                Title = $"Soovitus ({Math.Round(temp)}°C{(isRaining ? ", Vihm" : "")})",
                Items = finalItems
            };

            return (true, "Edu!", outfit);
        }
    }
}