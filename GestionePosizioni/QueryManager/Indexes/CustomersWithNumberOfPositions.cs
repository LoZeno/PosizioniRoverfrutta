using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.OData.Query.SemanticAst;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class CustomersWithNumberOfDocuments : AbstractMultiMapIndexCreationTask<CustomerRow>
    {
        public CustomersWithNumberOfDocuments()
        {
            AddMap<Customer>(
                customers => from customer in customers
                                    select new CustomerRow
                                    {
                                        Id = customer.Id,
                                        CompanyName = customer.CompanyName,
                                        NumberOfSalesConfirmations = 0,
                                        NumberOfLoadingDocuments = 0,
                                        NumberOfPriceConfirmations = 0
                                    });

            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                                     select new CustomerRow
                                     {
                                         Id = sc.Customer.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 1,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 0
                                     });
            AddMap<SaleConfirmation>(
                saleConfirmations => from sc in saleConfirmations
                                     select new CustomerRow
                                     {
                                         Id = sc.Provider.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 1,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 0
                                     }
                 );

            AddMap<LoadingDocument>(
                loadingDocuments => from ld in loadingDocuments
                                     select new CustomerRow
                                     {
                                         Id = ld.Customer.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 1,
                                         NumberOfPriceConfirmations = 0
                                     });
            AddMap<LoadingDocument>(
                loadingDocuments => from ld in loadingDocuments
                                     select new CustomerRow
                                     {
                                         Id = ld.Provider.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 1,
                                         NumberOfPriceConfirmations = 0
                                     }
                 );

            AddMap<PriceConfirmation>(
                priceConfirmations => from pc in priceConfirmations
                                     select new CustomerRow
                                     {
                                         Id = pc.Customer.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 1
                                     });
            AddMap<PriceConfirmation>(
                priceConfirmations => from pc in priceConfirmations
                                     select new CustomerRow
                                     {
                                         Id = pc.Provider.Id,
                                         CompanyName = string.Empty,
                                         NumberOfSalesConfirmations = 0,
                                         NumberOfLoadingDocuments = 0,
                                         NumberOfPriceConfirmations = 1
                                     }
                 );


            Reduce = rows => from row in rows
                group row by new {Id = row.Id}
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
