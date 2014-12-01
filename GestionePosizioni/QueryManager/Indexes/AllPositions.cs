using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class AllPositions : AbstractMultiMapIndexCreationTask<PositionsListRow>
    {
        public AllPositions()
        {
            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations 
                                     select 
                                     new PositionsListRow
                                     {
                                         DocumentId = sc.Id,
                                         CustomerName = sc.Customer.CompanyName,
                                         DocumentDate = sc.DocumentDate,
                                         ShippingDate = sc.ShippingDate,
                                         ProgressiveNumber = int.Parse(sc.Id.Split('/')[1]),
                                         HasLoadingDocument = false,
                                         HasPriceConfirmation = false,
                                         ProviderName = sc.Provider.CompanyName,
                                     });
            AddMap<LoadingDocument>(
                loadingDocuments => from ld in loadingDocuments
                                    select 
                                    new PositionsListRow
                                    {
                                        DocumentId = ld.Id,
                                        CustomerName = ld.Customer.CompanyName,
                                        DocumentDate = ld.DocumentDate,
                                        ShippingDate = ld.ShippingDate,
                                        ProgressiveNumber = int.Parse(ld.Id.Split('/')[1]),
                                        HasLoadingDocument = true,
                                        HasPriceConfirmation = false,
                                        ProviderName = ld.Provider.CompanyName,
                                    });
            AddMap<PriceConfirmation>(
                priceConfirmations => from pc in priceConfirmations
                                      select 
                                      new PositionsListRow
                                      {
                                          DocumentId = pc.Id,
                                          CustomerName = pc.Customer.CompanyName,
                                          DocumentDate = pc.DocumentDate,
                                          ShippingDate = pc.ShippingDate,
                                          ProgressiveNumber = int.Parse(pc.Id.Split('/')[1]),
                                          HasLoadingDocument = true,
                                          HasPriceConfirmation = true,
                                          ProviderName = pc.Provider.CompanyName,
                                      });

            Reduce = positions => from position in positions 
                                       group position by position.ProgressiveNumber
                                       into wholeDocument
                                       select 
                                       new PositionsListRow
                                       {
                                           DocumentId = "Positions/" + wholeDocument.Key,
                                           CustomerName = wholeDocument.Max(d => d.CustomerName),
                                           DocumentDate = wholeDocument.Max(d => d.DocumentDate),
                                           HasLoadingDocument = wholeDocument.Max(d => d.HasLoadingDocument),
                                           HasPriceConfirmation = wholeDocument.Max(d => d.HasPriceConfirmation),
                                           ProgressiveNumber = wholeDocument.Key,
                                           ProviderName = wholeDocument.Max(d => d.ProviderName),
                                           ShippingDate = wholeDocument.Max(d => d.ShippingDate),
                                       };

            Index(plr => plr.ProgressiveNumber, Raven.Abstractions.Indexing.FieldIndexing.Default);
            StoreAllFields(Raven.Abstractions.Indexing.FieldStorage.Yes);
        }
    }
}
