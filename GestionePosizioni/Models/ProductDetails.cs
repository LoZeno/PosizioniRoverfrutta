namespace Models
{
    public class ProductDetails
    {
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public int Pallets { get; set; }
        public int Packages { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal PriceParameter { get; set; }
    }
}