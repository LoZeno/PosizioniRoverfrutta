using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;
using System.Linq;

namespace QueryManager.Indexes
{
    public class TransportersWithNumberOfDocuments : AbstractMultiMapIndexCreationTask<CustomerRow>
    {
        public TransportersWithNumberOfDocuments()
        {
            AddMap<Transporter>(
                transporters => from transporter in transporters
                                select new CustomerRow
                                {
                                    Id = transporter.Id,
                                    CompanyName = transporter.CompanyName,
                                    NumberOfSalesConfirmations = 0,
                                    NumberOfLoadingDocuments = 0,
                                    NumberOfPriceConfirmations = 0
                                });

            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                                     select new CustomerRow
                                     {
                                         Id = sc.Transporter.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 1,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 0
                                     });

            AddMap<LoadingDocument>(
                saleConfirmations => from sc in saleConfirmations
                                     select new CustomerRow
                                     {
                                         Id = sc.Transporter.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 1,
                                         NumberOfPriceConfirmations = 0
                                     });

            AddMap<PriceConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                                     select new CustomerRow
                                     {
                                         Id = sc.Transporter.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 1
                                     });

            Reduce = rows => from row in rows
                             group row by new { Id = row.Id }
                into summaryRow
                             select new CustomerRow
                             {
                                 Id = summaryRow.Key.Id,
                                 CompanyName = summaryRow.Max(sr => sr.CompanyName),
                                 NumberOfSalesConfirmations = summaryRow.Sum(sr => sr.NumberOfSalesConfirmations),
                                 NumberOfLoadingDocuments = summaryRow.Sum(sr => sr.NumberOfLoadingDocuments),
                                 NumberOfPriceConfirmations = summaryRow.Sum(sr => sr.NumberOfPriceConfirmations),
                             };
        }
    }
}
