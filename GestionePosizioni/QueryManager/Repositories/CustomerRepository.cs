using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class CustomerRepository : CompanyRepositoryBase<Customer>
    {
        public CustomerRepository(IDocumentSession session) : base(session)
        {
        }
    }
}
