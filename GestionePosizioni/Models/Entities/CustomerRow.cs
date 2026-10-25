namespace Models.Entities;

public class CustomerRow
{
    public string Id { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int NumberOfSalesConfirmations { get; set; }
    public int NumberOfLoadingDocuments { get; set; }
    public int NumberOfPriceConfirmations { get; set; }
}
