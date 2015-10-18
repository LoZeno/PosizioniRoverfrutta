namespace Models.Entities
{
    public class CustomerRow
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfSalesConfirmations { get; set; }
        public int NumberOfLoadingDocuments { get; set; }
        public int NumberOfPriceConfirmations { get; set; }
    }
}
