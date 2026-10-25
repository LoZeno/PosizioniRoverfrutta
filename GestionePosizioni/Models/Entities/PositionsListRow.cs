using System;

namespace Models.Entities;

public class PositionsListRow
{
    public string DocumentId { get; set; } = string.Empty;
    public int ProgressiveNumber { get; set; }
    public DateTime? DocumentDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public bool HasLoadingDocument { get; set; }
    public bool HasPriceConfirmation { get; set; }
}