using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class ProviderRepository : CompanyRepositoryBase<Provider>
    {
        public ProviderRepository(IDocumentSession session)
            : base(session)
        {
        }
    }
}
