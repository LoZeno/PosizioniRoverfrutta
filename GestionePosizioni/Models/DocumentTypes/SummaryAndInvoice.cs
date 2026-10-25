using System;
using System.Collections.Generic;
using Models.Companies;
using Models.Entities;

namespace Models.DocumentTypes;

public class SummaryAndInvoice
{
    public string Id { get; set; }

    public int? InvoiceNumber
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Id))
                return DocumentBase.TryParseIdNumber(Id);
            return null;
        }
        set => Id = value.HasValue ? "SummaryAndInvoices/" + value : "SummaryAndInvoices/";
    }

    public Customer Customer { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public string StartDateString => StartDate.HasValue ? StartDate.Value.ToLongDateString() : string.Empty;
    public DateTime? EndDate { get; set; }
    public string EndDateString => EndDate.HasValue ? EndDate.Value.ToLongDateString() : string.Empty;
    public bool IncludeOpenPositions { get; set; }
    public List<SummaryRow> SummaryRows { get; set; } = [];
    public decimal CommissionsTotal { get; set; }
    public decimal InvoiceVat { get; set; }
    public decimal CalculatedInvoiceVat { get; set; }
    public decimal TaxedAmount { get; set; }
    public decimal Witholding { get; set; }
    public decimal CalculatedWitholding { get; set; }
    public decimal NetAmount { get; set; }
    public string Base64Logo { get; set; } = string.Empty;
    public DateTime? InvoiceDate { get; set; }
    public string InvoiceDateString => InvoiceDate.HasValue ? InvoiceDate.Value.ToShortDateString() : string.Empty;
    public List<PartialByCompanyName> PartialsByCompanyName { get; set; } = [];
}