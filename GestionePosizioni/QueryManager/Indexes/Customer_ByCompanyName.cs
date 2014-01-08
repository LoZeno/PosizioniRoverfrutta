using System.Linq;
using Models;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
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
}
