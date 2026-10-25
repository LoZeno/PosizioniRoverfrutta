using System.Linq;
using Models.Companies;
using Raven.Client.Documents.Indexes;

namespace QueryManager.Indexes;

public class Customer_ByCompanyName : AbstractIndexCreationTask<Customer>
{
    public Customer_ByCompanyName()
    {
        Map = customers => from customer in customers
            select new
            {
                customer.CompanyName
            };
    }
}
