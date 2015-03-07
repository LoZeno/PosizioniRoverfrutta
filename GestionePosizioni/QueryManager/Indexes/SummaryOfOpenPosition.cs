using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class SummaryOfOpenPosition : AbstractMultiMapIndexCreationTask<SummaryRow>
    {
        public SummaryOfOpenPosition()
        {
            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                    where sc.CustomerCommission.HasValue && sc.CustomerCommission.Value > 0
                    select
                        new SummaryRow
                        {
                            Commission = sc.CustomerCommission.Value,
                            InvoiceCustomerId = sc.Customer.Id,
                            CompanyName = sc.Provider.CompanyName,
                            DocumentId = int.Parse(sc.Id.Split('/')[1]),
                            ShippingDate = sc.ShippingDate,
                            TaxableAmount = 0,
                            TransportDocument = string.Empty,
                            CanMakeInvoice = false
                        });
            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                    where sc.ProviderCommission.HasValue && sc.ProviderCommission.Value > 0
                    select
                        new SummaryRow
                        {
                            Commission = sc.ProviderCommission.Value,
                            InvoiceCustomerId = sc.Provider.Id,
                            CompanyName = sc.Customer.CompanyName,
                            DocumentId = int.Parse(sc.Id.Split('/')[1]),
                            ShippingDate = sc.ShippingDate,
                            TaxableAmount = 0,
                            TransportDocument = string.Empty,
                            CanMakeInvoice = false
                        });

            Reduce = positions => from position in positions
                group position by new { position.DocumentId, position.InvoiceCustomerId }
                into summaryRow
                select new SummaryRow
                {
                    InvoiceCustomerId = summaryRow.Key.InvoiceCustomerId,
                    Commission = summaryRow.ElementAt(0).Commission,
                    CompanyName = summaryRow.ElementAt(0).CompanyName,
                    DocumentId = summaryRow.Key.DocumentId,
                    ShippingDate = summaryRow.ElementAt(0).ShippingDate,
                    TaxableAmount = summaryRow.ElementAt(0).TaxableAmount,
                    TransportDocument = summaryRow.ElementAt(0).TransportDocument,
                    CanMakeInvoice = false
                };
        }
    }
}