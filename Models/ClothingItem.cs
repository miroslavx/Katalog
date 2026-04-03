using SQLite;

namespace Katalog.Models
{
    public class ClothingItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;    // Верх, Низ, Обувь
        public string SubCategory { get; set; } = string.Empty; // Конкретно: Футболка, Джинсы и т.д.

        public string PresetType { get; set; } = string.Empty;
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}