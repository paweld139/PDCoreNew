namespace PDCoreNew.Models.Shop
{
    public class Item
    {
        public Item(string articleId, string name, decimal price)
        {
            ArticleId = articleId;
            Name = name;
            Price = price;
        }

        public string ArticleId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
