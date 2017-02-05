using System;
using System.Collections.Generic;
using Models.Companies;
using Models.Entities;

namespace Models.DocumentTypes
{
    public class SummaryAndInvoice
    {
        public SummaryAndInvoice()
        {
            SummaryRows = new List<SummaryRow>();
            PartialsByCompanyName = new List<PartialByCompanyName>();
        }

        public string Id { get; set; }

        public int? InvoiceNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    var strings = Id.Split('/');
                    if (strings.Length > 1 && !string.IsNullOrWhiteSpace(strings[1]))
                        return int.Parse(strings[1]);
                    return 0;
                }
                return null;
            }
            set { Id = value.HasValue ? "SummaryAndInvoices/" + value : "SummaryAndInvoices/"; }
        }

        public Customer Customer { get; set; }
        public DateTime? StartDate { get; set; }

        public string StartDateString
        {
            get
            {
                return StartDate.HasValue ? StartDate.Value.ToLongDateString() : string.Empty;
            }
        }

        public DateTime? EndDate { get; set; }

        public string EndDateString
        {
            get
            {
                return EndDate.HasValue ? EndDate.Value.ToLongDateString() : string.Empty;
            }
        }

        public bool IncludeOpenPositions { get; set; }
        public List<SummaryRow> SummaryRows { get; set; }
        public decimal CommissionsTotal { get; set; }
        public decimal InvoiceVat { get; set; }
        public decimal CalculatedInvoiceVat { get; set; }
        public decimal TaxedAmount { get; set; }
        public decimal Witholding { get; set; }
        public decimal CalculatedWitholding { get; set; }
        public decimal NetAmount { get; set; }

        public string Base64Logo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceDateString
        {
            get
            {
                return InvoiceDate.HasValue ? InvoiceDate.Value.ToShortDateString() : string.Empty;
            }
        }

        public List<PartialByCompanyName> PartialsByCompanyName { get; set; }
    }
}