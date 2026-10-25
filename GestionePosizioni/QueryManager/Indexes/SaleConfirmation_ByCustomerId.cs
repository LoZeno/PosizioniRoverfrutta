using System.Linq;
using Models.DocumentTypes;
using Raven.Client.Documents.Indexes;

namespace QueryManager.Indexes;

public class SaleConfirmation_ByCustomerId : AbstractIndexCreationTask<SaleConfirmation>
{
    public SaleConfirmation_ByCustomerId()
    {
        Map = saleconfirmations => from saleconfirmation in saleconfirmations
            select new
            {
                Customer_Id = saleconfirmation.Customer.Id
            };
    }
}
