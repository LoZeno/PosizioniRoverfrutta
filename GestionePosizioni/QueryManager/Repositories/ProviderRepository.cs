using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class ProviderRepository : RepositoryBase<Provider>
    {
        public ProviderRepository(IDocumentSession session)
            : base(session)
        {
        }

        public override string Add(Provider entity)
        {
            _session.Store(entity);
            return entity.Id;
        }

        public IEnumerable<Provider> FindByPartialName(string partialName)
        {
            return _session.Query<Provider>("Provider/ByCompanyName")
                .Search(c => c.CompanyName, "*" + partialName + "*", escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
        }
    }
}
