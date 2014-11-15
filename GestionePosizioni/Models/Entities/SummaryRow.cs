using System;

namespace Models.Entities
{
    public class SummaryRow
    {
        public int DocumentId { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string DocumentDateString
        {
            get
            {
                return ShippingDate.HasValue ? ShippingDate.Value.ToString("m") : string.Empty;
            }
        }
        public string TransportDocument { get; set; }
        public string CompanyName { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal PayableAmount { get; set; }
    }
}
