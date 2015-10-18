using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class ProductsWithNumberOfDocuments : AbstractMultiMapIndexCreationTask<ProductRow>
    {
        public ProductsWithNumberOfDocuments()
        {
            AddMap<ProductDescription>(
                productDescriptions => from product in productDescriptions
                    select new ProductRow
                    {
                        Id = product.Id.ToString(),
                        Description = product.Description,
                        NumberOfSalesConfirmations = 0,
                        NumberOfLoadingDocuments = 0,
                        NumberOfPriceConfirmations = 0
                    });

            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                    from singleProduct in sc.ProductDetails
                    select new ProductRow
                    {
                        Id = singleProduct.ProductId.ToString(),
                        Description = singleProduct.Description,
                        NumberOfSalesConfirmations = 1,
                        NumberOfLoadingDocuments = 0,
                        NumberOfPriceConfirmations = 0
                    });

            AddMap<LoadingDocument>(
                loadingDocuments => from ld in loadingDocuments
                    from singleProduct in ld.ProductDetails
                    select new ProductRow
                    {
                        Id = singleProduct.ProductId.ToString(),
                        Description = singleProduct.Description,
                        NumberOfSalesConfirmations = 0,
                        NumberOfLoadingDocuments = 1,
                        NumberOfPriceConfirmations = 0
                    });

            AddMap<PriceConfirmation>(
                priceConfirmations => from pc in priceConfirmations
                    from singleProduct in pc.ProductDetails
                    select new ProductRow
                    {
                        Id = singleProduct.ProductId.ToString(),
                        Description = singleProduct.Description,
                        NumberOfSalesConfirmations = 0,
                        NumberOfLoadingDocuments = 0,
                        NumberOfPriceConfirmations = 1
                    });



            Reduce = rows => from row in rows
                group row by new { Id = row.Id }
                into summaryRow
                select new ProductRow
                {
                    Id = summaryRow.Key.Id,
                    Description = summaryRow.Max(sr => sr.Description),
                    NumberOfSalesConfirmations = summaryRow.Sum(sr => sr.NumberOfSalesConfirmations),
                    NumberOfLoadingDocuments = summaryRow.Sum(sr => sr.NumberOfLoadingDocuments),
                    NumberOfPriceConfirmations = summaryRow.Sum(sr => sr.NumberOfPriceConfirmations),
                };
        }
    }
}