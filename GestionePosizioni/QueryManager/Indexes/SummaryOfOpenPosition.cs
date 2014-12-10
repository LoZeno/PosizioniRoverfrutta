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
                saleConfirmations => from ld in saleConfirmations
                    where ld.CustomerCommission.HasValue
                    select
                        new SummaryRow
                        {
                            Commission = ld.CustomerCommission.Value,
                            InvoiceCustomerId = ld.Customer.Id,
                            CompanyName = ld.Provider.CompanyName,
                            DocumentId = int.Parse(ld.Id.Split('/')[1]),
                            ShippingDate = ld.ShippingDate,
                            TaxableAmount = 0,
                            TransportDocument = string.Empty,
                            CanMakeInvoice = false
                        });
            AddMap<SaleConfirmation>(
                saleConfirmations => from pc in saleConfirmations
                    where pc.ProviderCommission.HasValue
                    select
                        new SummaryRow
                        {
                            Commission = pc.ProviderCommission.Value,
                            InvoiceCustomerId = pc.Provider.Id,
                            CompanyName = pc.Customer.CompanyName,
                            DocumentId = int.Parse(pc.Id.Split('/')[1]),
                            ShippingDate = pc.ShippingDate,
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