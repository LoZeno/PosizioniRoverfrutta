using System;
using System.Collections.Generic;
using Models.Companies;

namespace Models.DocumentTypes;

public abstract class DocumentBase
{
    protected DocumentBase()
    {
        ProductDetails = [];
        Provider = new Customer();
        Customer = new Customer();
        Transporter = new Transporter();
        DocumentDate = DateTime.Today.Date;
        Notes = "Le coordinate bancarie verranno indicate sulla fattura.";
    }

    public string? Id { get; set; }

    public int ProgressiveNumber
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Id))
                return 0;
            return TryParseIdNumber(Id);
        }
    }

    internal static int TryParseIdNumber(string id)
    {
        var strings = id.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (strings.Length < 2)
            return 0;
        var last = strings[^1];
        var dash = last.IndexOf('-');
        if (dash >= 0)
            last = last[..dash];
        return int.TryParse(last, out var n) ? n : 0;
    }

    public Customer Customer { get; set; }
    public Customer Provider { get; set; }
    public Transporter Transporter { get; set; }
    public DateTime? DocumentDate { get; set; }

    public string DocumentDateString =>
        DocumentDate.HasValue ? DocumentDate.Value.ToShortDateString() : string.Empty;

    public DateTime? ShippingDate { get; set; }

    public string ShippingDateString =>
        ShippingDate.HasValue ? ShippingDate.Value.ToShortDateString() : string.Empty;

    public DateTime? DeliveryDate { get; set; }

    public string DeliveryDateString =>
        DeliveryDate.HasValue ? DeliveryDate.Value.ToShortDateString() : string.Empty;

    public string? TruckLicensePlate { get; set; }
    public decimal? Rental { get; set; }
    public string? DeliveryEx { get; set; }
    public string? TermsOfPayment { get; set; }
    public decimal? InvoiceDiscount { get; set; }
    public decimal? CustomerCommission { get; set; }
    public decimal? ProviderCommission { get; set; }
    public string? Notes { get; set; }
    public string? Lot { get; set; }
    public string? OrderCode { get; set; }
    public List<ProductDetails> ProductDetails { get; set; }
}
