using System;

namespace Models.Entities
{
    public class PositionsListRow
    {
        public string DocumentId { get; set; }

        public int ProgressiveNumber
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DocumentId))
                {
                    var strings = DocumentId.Split('/');
                    if (strings.Length > 1 && !string.IsNullOrWhiteSpace(strings[1]))
                        return int.Parse(strings[1]);
                    return 0;
                }
                return 0;
            }
        }
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
        public bool HasPriceConfirmation { get; set; }
    }
}