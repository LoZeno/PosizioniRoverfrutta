using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class CustomerRepository : CompanyRepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDocumentSession session) : base(session)
        {
        }
    }
}
