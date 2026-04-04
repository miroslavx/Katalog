namespace Katalog.Models
{
    public class OutfitSet
    {
        public string Title { get; set; } = string.Empty; 
        public List<ClothingItem> Items { get; set; } = new();
    }
}