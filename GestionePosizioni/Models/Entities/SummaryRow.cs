using System;

namespace Models.Entities;

public class SummaryRow
{
    public int DocumentId { get; set; }
    public string InvoiceCustomerId { get; set; } = string.Empty;
    public DateTime? ShippingDate { get; set; }
    public string DocumentDateString => ShippingDate.HasValue ? ShippingDate.Value.ToString("m") : string.Empty;
    public string TransportDocument { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public decimal Commission { get; set; }
    public decimal PayableAmount { get; set; }
    public bool CanMakeInvoice { get; set; }
}
