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

        public async Task<(bool Success, string Message, List<OutfitSet>? Outfits)> GenerateOutfitsByStyleAsync(string style)
        {
            var wardrobe = await _dbService.GetWardrobeAsync();
            List<string> allowedTops = new();
            List<string> allowedBottoms = new();
            List<string> allowedShoes = new();
            List<string> allowedAccessories = new();
            List<string> allowedOuterwear = new();

            switch (style)
            {
                case "Ametlik":
                    allowedTops = new List<string> { "Särk", "Pintsak" };
                    allowedBottoms = new List<string> { "Püksid", "Seelik" };
                    allowedShoes = new List<string> { "Kingad" };
                    allowedAccessories = new List<string> { "Lips", "Vöö", "Käekell" };
                    break;
                case "Suvine sport":
                    allowedTops = new List<string> { "T-särk", "Polo" };
                    allowedBottoms = new List<string> { "Lühikesed püksid", "Dressipüksid" };
                    allowedShoes = new List<string> { "Tossud" };
                    allowedAccessories = new List<string> { "Müts", "Käekell", "Sokid", "Aluspesu" };
                    break;
                case "Talvine sport":
                    allowedTops = new List<string> { "Pusa", "Kampsun" };
                    allowedBottoms = new List<string> { "Dressipüksid", "Püksid" };
                    allowedOuterwear = new List<string> { "Jope", "Tuulepluus", "Vest" };
                    allowedShoes = new List<string> { "Tossud", "Saapad" };
                    allowedAccessories = new List<string> { "Müts", "Kindad", "Sall", "Sokid", "Aluspesu" };
                    break;
                case "Pidu":
                    allowedTops = new List<string> { "Särk", "Polo", "Pusa" };
                    allowedBottoms = new List<string> { "Teksad", "Püksid", "Seelik" };
                    allowedShoes = new List<string> { "Kingad", "Tossud" };
                    break;
                case "Igapäevane":
                    allowedTops = new List<string> { "T-särk", "Kampsun", "Pusa" };
                    allowedBottoms = new List<string> { "Teksad" };
                    allowedShoes = new List<string> { "Tossud", "Saapad" };
                    break;
                case "Magamiseks":
                    allowedTops = new List<string> { "T-särk", "Pidžaama särk" };
                    allowedBottoms = new List<string> { "Lühikesed püksid", "Pidžaama püksid" };
                    allowedShoes = new List<string> { "Sussid" };
                    break;
            }

            var tops = wardrobe.Where(x => allowedTops.Contains(x.SubCategory)).ToList();
            var outerwears = wardrobe.Where(x => allowedOuterwear.Contains(x.SubCategory)).ToList();
            var bottoms = wardrobe.Where(x => allowedBottoms.Contains(x.SubCategory)).ToList();
            var shoes = wardrobe.Where(x => allowedShoes.Contains(x.SubCategory)).ToList();
            var accs = wardrobe.Where(x => allowedAccessories.Contains(x.SubCategory)).ToList();

            if (!tops.Any() || !bottoms.Any())
            {
                return (false, $"Sul puuduvad vajalikud riided ({style} stiil). Lisa kappi sobiv ülemine või alumine osa!", null);
            }

            var outfits = new List<OutfitSet>();
            var rand = new Random();

            tops = tops.OrderBy(x => rand.Next()).ToList();
            bottoms = bottoms.OrderBy(x => rand.Next()).ToList();

            int variations = Math.Min(3, Math.Max(tops.Count, bottoms.Count));

            for (int i = 0; i < variations; i++)
            {
                var currentSet = new List<ClothingItem>
                {
                    tops[i % tops.Count],
                    bottoms[i % bottoms.Count]
                };

                if (outerwears.Any()) currentSet.Add(outerwears[rand.Next(outerwears.Count)]);
                if (shoes.Any()) currentSet.Add(shoes[rand.Next(shoes.Count)]);
                if (accs.Any() && rand.Next(2) == 0) currentSet.Add(accs[rand.Next(accs.Count)]);
                outfits.Add(new OutfitSet { Title = $"Variant {i + 1}", Items = currentSet });
            }

            return (true, "Valmis!", outfits);
        }


        public async Task<(bool Success, string Message, OutfitSet? Outfit)> GenerateOutfitByWeatherAsync(WeatherResponse weather)
        {
            var wardrobe = await _dbService.GetWardrobeAsync();
            var rand = new Random();

            double temp = weather.Main.Temp;
            bool isRaining = weather.Weather.Any(w => w.Main.Contains("Rain", StringComparison.OrdinalIgnoreCase) || w.Main.Contains("Drizzle", StringComparison.OrdinalIgnoreCase));

            List<string> allowedTops = new();
            List<string> allowedBottoms = new();
            List<string> allowedOuterwear = new();
            List<string> allowedShoes = new();

            if (temp >= 20)
            {
                allowedTops.AddRange(new[] { "T-särk", "Polo", "Särk" });
                allowedBottoms.AddRange(new[] { "Lühikesed püksid", "Seelik" });
                allowedShoes.AddRange(new[] { "Sandaalid", "Tossud" });
            }
            else if (temp >= 10 && temp < 20)
            {
                allowedTops.AddRange(new[] { "Särk", "T-särk" });
                allowedBottoms.AddRange(new[] { "Teksad", "Püksid", "Seelik" });
                allowedOuterwear.AddRange(new[] { "Tagi", "Tuulepluus", "Pusa", "Kampsun" });
                allowedShoes.AddRange(new[] { "Tossud", "Kingad" });
            }
            else if (temp >= 0 && temp < 10)
            {
                allowedTops.AddRange(new[] { "Pusa", "Kampsun", "Särk" });
                allowedBottoms.AddRange(new[] { "Teksad", "Püksid" });
                allowedOuterwear.AddRange(new[] { "Jope", "Mantel" });
                allowedShoes.AddRange(new[] { "Saapad", "Tossud" });
            }
            else
            {
                allowedTops.AddRange(new[] { "Kampsun", "Pusa" });
                allowedBottoms.AddRange(new[] { "Teksad", "Dressipüksid" });
                allowedOuterwear.AddRange(new[] { "Jope" });
                allowedShoes.AddRange(new[] { "Saapad" });
            }

            if (isRaining)
            {
                allowedOuterwear.Clear();
                allowedOuterwear.AddRange(new[] { "Tuulepluus", "Jope", "Mantel", "Tagi" });
                allowedShoes.Remove("Sandaalid");
                if (!allowedShoes.Any()) allowedShoes.Add("Saapad");
            }

            var tops = wardrobe.Where(x => allowedTops.Contains(x.SubCategory)).ToList();
            var bottoms = wardrobe.Where(x => allowedBottoms.Contains(x.SubCategory)).ToList();
            var outerwears = wardrobe.Where(x => allowedOuterwear.Contains(x.SubCategory)).ToList();
            var shoes = wardrobe.Where(x => allowedShoes.Contains(x.SubCategory)).ToList();

            if (!tops.Any() || !bottoms.Any())
            {
                return (false, "Sul pole selle ilma jaoks sobivaid baasriideid (ülemine või alumine osa).", null);
            }

            var finalItems = new List<ClothingItem>
            {
                tops[rand.Next(tops.Count)],
                bottoms[rand.Next(bottoms.Count)]
            };

            if (outerwears.Any() && (temp < 20 || isRaining))
            {
                finalItems.Add(outerwears[rand.Next(outerwears.Count)]);
            }

            if (shoes.Any())
            {
                finalItems.Add(shoes[rand.Next(shoes.Count)]);
            }

            var outfit = new OutfitSet
            {
                Title = $"Soovitus ({Math.Round(temp)}°C{(isRaining ? ", Vihm" : "")})",
                Items = finalItems
            };

            return (true, "Edu!", outfit);
        }
    }
}