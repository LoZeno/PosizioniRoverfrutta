using System;

namespace Models.Entities
{
    public class PositionsListRow
    {
        public int DocumentId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string DocumentDateString
        {
            get
            {
                return DocumentDate.HasValue ? DocumentDate.Value.ToString("m") : string.Empty;
            }
        }
        public DateTime? ShippingDate { get; set; }
        public string ShippingDateString
        {
            get
            {
                return ShippingDate.HasValue ? ShippingDate.Value.ToString("m") : string.Empty;
            }
        }
        public string CustomerName { get; set; }
        public string ProviderName { get; set; }
        public decimal TaxableAmount { get; set; }
        public bool HasLoadingDocument { get; set; }
        public bool HasProceConfirmation { get; set; }
    }
}