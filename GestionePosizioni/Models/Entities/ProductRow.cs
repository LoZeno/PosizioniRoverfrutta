namespace Models.Entities;

public class ProductRow
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumberOfSalesConfirmations { get; set; }
    public int NumberOfLoadingDocuments { get; set; }
    public int NumberOfPriceConfirmations { get; set; }
}