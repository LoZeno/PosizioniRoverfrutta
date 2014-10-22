using System;

namespace Models.Entities
{
    public class SummaryRow
    {
        public int DocumentId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string TransportDocument { get; set; }
        public string CompanyName { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
