namespace Models.Entities
{
    public class ProductRow
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int NumberOfSalesConfirmations { get; set; }
        public int NumberOfLoadingDocuments { get; set; }
        public int NumberOfPriceConfirmations { get; set; }
    }
}