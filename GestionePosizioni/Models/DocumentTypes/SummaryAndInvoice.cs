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
        public List<SummaryRow> SummaryRows { get; set; }
        public decimal CommissionsTotal { get; set; }
        public decimal InvoiceVat { get; set; }
        public decimal CalculatedInvoiceVat { get; set; }
        public decimal TaxedAmount { get; set; }
        public decimal Witholding { get; set; }
        public decimal CalculatedWitholding { get; set; }
        public decimal NetAmount { get; set; }

        public string Base64Logo { get; set; }
        public int? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceDateString
        {
            get
            {
                return InvoiceDate.HasValue ? InvoiceDate.Value.ToShortDateString() : string.Empty;
            }
        }
    }
}