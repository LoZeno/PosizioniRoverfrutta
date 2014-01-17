using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var terms = partialName.Split(' ');
            var searchString = new StringBuilder("*");
            foreach (var name in partialName)
            {
                searchString.Append(name);
                searchString.Append("*");
            }

            string objectName = typeof (T).Name;
            return _session.Query<T>(objectName + "/ByCompanyName")
                    .Search(c => c.CompanyName, searchString.ToString(), options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
        }

        public IEnumerable<string> GetCompaniesForListBox(string partialName)
        {
            return FindByPartialName(partialName).Select(x => x.CompanyName);
        }
    }
}