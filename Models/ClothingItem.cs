using SQLite;

namespace Katalog.Models
{
    public class ClothingItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } // Например: "Любимая синяя рубашка"

        public string Category { get; set; } // Верх, Низ, Обувь (Top, Bottom, Shoes)

        public string PresetType { get; set; } // Casual, Formal, Sport

        public int MinTemp { get; set; } // Например, +15
        public int MaxTemp { get; set; } // Например, +25

        public string ImagePath { get; set; } // Путь к фото на телефоне
    }
}