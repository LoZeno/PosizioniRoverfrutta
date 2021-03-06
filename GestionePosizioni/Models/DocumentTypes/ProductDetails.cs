namespace Models.DocumentTypes
{
    public class ProductDetails
    {
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal Pallets { get; set; }
        public string PalletType { get; set; }
        public int Packages { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal PriceParameter { get; set; }

        public decimal TotalPrice { get { return Price*PriceParameter; } }
    }
}