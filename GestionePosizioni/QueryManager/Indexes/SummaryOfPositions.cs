using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Documents.Indexes;

namespace QueryManager.Indexes;

public class SummaryOfPositions : AbstractMultiMapIndexCreationTask<SummaryRow>
{
    public SummaryOfPositions()
    {
        AddMap<PriceConfirmation>(saleConfirmations => from pc in saleConfirmations
            where pc.CustomerCommission.HasValue && pc.CustomerCommission.Value > 0
            select new SummaryRow
            {
                Commission = pc.CustomerCommission.Value,
                InvoiceCustomerId = pc.Customer.Id,
                CompanyName = pc.Provider.CompanyName,
                DocumentId = int.Parse(pc.Id.Split('/')[1].Split('-')[0]),
                ShippingDate = pc.ShippingDate,
                TaxableAmount = pc.TaxableAmount,
                TransportDocument = pc.TransportDocument,
                CanMakeInvoice = true
            });
        AddMap<PriceConfirmation>(saleConfirmations => from pc in saleConfirmations
            where pc.ProviderCommission.HasValue && pc.ProviderCommission.Value > 0
            select new SummaryRow
            {
                Commission = pc.ProviderCommission.Value,
                InvoiceCustomerId = pc.Provider.Id,
                CompanyName = pc.Customer.CompanyName,
                DocumentId = int.Parse(pc.Id.Split('/')[1].Split('-')[0]),
                ShippingDate = pc.ShippingDate,
                TaxableAmount = pc.TaxableAmount,
                TransportDocument = pc.TransportDocument,
                CanMakeInvoice = true
            });

        Reduce = positions => from position in positions
            group position by new { position.DocumentId, position.InvoiceCustomerId }
            into summaryRow
            select new SummaryRow
            {
                InvoiceCustomerId = summaryRow.Key.InvoiceCustomerId,
                Commission = summaryRow.Max(x => x.Commission),
                CompanyName = summaryRow.Max(x => x.CompanyName),
                DocumentId = summaryRow.Key.DocumentId,
                ShippingDate = summaryRow.Max(x => x.ShippingDate),
                TaxableAmount = summaryRow.Max(x => x.TaxableAmount),
                TransportDocument = summaryRow.Max(x => x.TransportDocument),
                CanMakeInvoice = true
            };
    }
}
