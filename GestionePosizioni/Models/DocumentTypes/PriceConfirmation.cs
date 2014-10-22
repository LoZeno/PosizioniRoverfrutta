namespace Models.DocumentTypes
{
    public class PriceConfirmation : DocumentBase
    {
        public string TransportDocument { get; set; }
        public decimal TotalAmount { get; set; }
        public string GrandTotalString {get { return TotalAmount.ToString("F2"); }}
        public decimal CalculatedDiscount { get; set; }
        public string CalculatedDiscountString { get { return CalculatedDiscount.ToString("F2"); }}
        public decimal TaxableAmount { get; set; }
        public string TaxableAmountString {get { return TaxableAmount.ToString("F2"); }}
        public decimal Vat { get; set; }
        public decimal CalculatedVat { get; set; }
        public string CalculatedVatString {get { return CalculatedVat.ToString("F2"); }}
        public decimal FinalTotal { get; set; }
        public string FinalTotalString { get { return FinalTotal.ToString("F2"); } }
    }
}