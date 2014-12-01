using System;

namespace Models.Entities
{
    public class PositionsListRow
    {
        public string DocumentId { get; set; }
        public int ProgressiveNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string CustomerName { get; set; }
        public string ProviderName { get; set; }
        public decimal TaxableAmount { get; set; }
        public bool HasLoadingDocument { get; set; }
        public bool HasPriceConfirmation { get; set; }
    }
}