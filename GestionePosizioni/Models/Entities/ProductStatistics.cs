namespace Models.Entities
{
    public class ProductStatistics
    {
        public int? ProductId { get; set; }
        public string Description { get; set; }
        public decimal NetWeight { get; set; }
        public decimal PriceSum { get; set; }
        public int Instances { get; set; }
        public decimal AveragePrice { get { return PriceSum / Instances; } }
        public decimal TotalAmount { get; set; }
    }
}
