using System.Collections.Generic;
using System.Linq;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{

    public abstract class CompanyRepositoryBase<T> : RepositoryBase<T> where T : CompanyBase
    {
        public CompanyRepositoryBase(IDocumentSession session)
            : base(session)
        {
        }

        public override string Add(T entity)
        {
            _session.Store(entity);
            return entity.Id;
        }

        public IEnumerable<T> FindByPartialName(string partialName)
        {
            string objectName = typeof (T).Name;
            var query = _session.Query<T>(objectName + "/ByCompanyName");
            return partialName.Split(' ').Aggregate(query, (current, term) => current.Search(c => c.CompanyName, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));
        }
    }
}