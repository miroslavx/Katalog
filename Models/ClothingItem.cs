using SQLite;
namespace Katalog.Models
{public class ClothingItem
    {[PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;   
        public string SubCategory { get; set; } = string.Empty;

        public string PresetType { get; set; } = string.Empty;
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public string ImagePath { get; set; } = string.Empty;}
}