namespace Models.DocumentTypes
{
    public class SaleConfirmation : DocumentBase
    {
    }

    public class LoadingDocument : DocumentBase
    {
        public string TransportDocument { get; set; }
    }
}
