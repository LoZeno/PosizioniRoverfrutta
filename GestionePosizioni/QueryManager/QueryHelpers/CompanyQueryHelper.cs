using System.Collections.Generic;
using System.Linq;
using Models.Companies;
using Raven.Client;

namespace QueryManager.QueryHelpers
{
    public static class CompanyQueryHelper
    {
        public static IEnumerable<T> FindByPartialName<T>(this IDocumentSession session, string partialName) where T : CompanyBase
        {
            var objectName = typeof(T).Name;
            var query = session.Query<T>(objectName + "/ByCompanyName");
            return partialName.Split(' ').Aggregate(query, (current, term) => current.Search(c => c.CompanyName, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)).ToList();
        }
    }
}