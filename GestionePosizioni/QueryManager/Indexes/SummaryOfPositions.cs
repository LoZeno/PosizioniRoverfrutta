using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class SummaryOfPositions : AbstractMultiMapIndexCreationTask<SummaryRow>
    {
        public SummaryOfPositions()
        {
            AddMap<PriceConfirmation>(
                saleConfirmations => from pc in saleConfirmations
                                     where pc.CustomerCommission.HasValue
                                     select 
                                     new SummaryRow
                                     {
                                         Commission = pc.CustomerCommission.Value,
                                         InvoiceCustomerId = pc.Customer.Id,
                                         CompanyName = pc.Provider.CompanyName,
                                         DocumentId = int.Parse(pc.Id.Split('/')[1]),
                                         ShippingDate = pc.ShippingDate,
                                         TaxableAmount = pc.TaxableAmount,
                                         TransportDocument = pc.TransportDocument,
                                     });
            AddMap<PriceConfirmation>(
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
                                         TaxableAmount = pc.TaxableAmount,
                                         TransportDocument = pc.TransportDocument,
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
                                         TransportDocument = summaryRow.ElementAt(0).TransportDocument
                                     };
        }
    }
}
