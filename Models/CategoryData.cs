namespace Katalog.Models
{
    public static class CategoryData
    {
        public static Dictionary<string, List<string>> Categories = new()
        {
            { "Ülemine osa", new List<string> { "T-särk", "Särk", "Polo", "Kampsun", "Pusa", "Pintsak", "Pidžaama särk" } },
            { "Alumine osa", new List<string> { "Teksad", "Püksid", "Lühikesed püksid", "Seelik", "Dressipüksid", "Pidžaama püksid" } },
            { "Üleriided", new List<string> { "Jope", "Mantel", "Tagi", "Vest", "Tuulepluus" } },
            { "Pesu ja sokid", new List<string> { "Aluspesu", "Sokid" } },
            { "Jalanõud", new List<string> { "Tossud", "Kingad", "Saapad", "Sandaalid", "Sussid" } },
            { "Aksessuaarid", new List<string> { "Lips", "Müts", "Sall", "Vöö", "Kindad", "Käekell" } }
        };
    }
}