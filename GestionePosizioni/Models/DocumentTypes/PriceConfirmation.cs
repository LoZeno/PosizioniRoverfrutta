namespace Models.DocumentTypes;

public class PriceConfirmation : DocumentBase
{
    public string TransportDocument { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string GrandTotalString => TotalAmount.ToString("F2");
    public decimal CalculatedDiscount { get; set; }
    public string CalculatedDiscountString => CalculatedDiscount.ToString("F2");
    public decimal TaxableAmount { get; set; }
    public string TaxableAmountString => TaxableAmount.ToString("F2");
    public decimal Vat { get; set; }
    public decimal CalculatedVat { get; set; }
    public string CalculatedVatString => CalculatedVat.ToString("F2");
    public decimal FinalTotal { get; set; }
    public string FinalTotalString => FinalTotal.ToString("F2");
}