using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>
    {
        public CustomerRepository(IDocumentSession session) : base(session)
        {
        }

        public override string Add(Customer entity)
        {
            _session.Store(entity);
            return entity.Id;
        }

        public IEnumerable<Customer> FindByPartialName(string partialName)
        {
            return _session.Query<Customer>("Customer/ByCompanyName")
                .Search(c => c.CompanyName, "*" + partialName + "*", escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
        }
    }
}
