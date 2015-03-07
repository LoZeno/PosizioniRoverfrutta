using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class SummaryOfPartialPositions : AbstractMultiMapIndexCreationTask<SummaryRow>
    {
        public SummaryOfPartialPositions()
        {
            AddMap<LoadingDocument>(
                saleConfirmations => from ld in saleConfirmations
                    where ld.CustomerCommission.HasValue && ld.CustomerCommission.Value > 0
                    select
                        new SummaryRow
                        {
                            Commission = ld.CustomerCommission.Value,
                            InvoiceCustomerId = ld.Customer.Id,
                            CompanyName = ld.Provider.CompanyName,
                            DocumentId = int.Parse(ld.Id.Split('/')[1]),
                            ShippingDate = ld.ShippingDate,
                            TaxableAmount = 0,
                            TransportDocument = ld.TransportDocument,
                            CanMakeInvoice = false
                        });
            AddMap<LoadingDocument>(
                saleConfirmations => from ld in saleConfirmations
                    where ld.ProviderCommission.HasValue && ld.ProviderCommission.Value > 0
                    select
                        new SummaryRow
                        {
                            Commission = ld.ProviderCommission.Value,
                            InvoiceCustomerId = ld.Provider.Id,
                            CompanyName = ld.Customer.CompanyName,
                            DocumentId = int.Parse(ld.Id.Split('/')[1]),
                            ShippingDate = ld.ShippingDate,
                            TaxableAmount = 0,
                            TransportDocument = ld.TransportDocument,
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