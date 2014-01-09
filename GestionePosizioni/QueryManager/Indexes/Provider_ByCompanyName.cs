using System.Linq;
using Models;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class Provider_ByCompanyName : AbstractIndexCreationTask<Provider>
    {
        public Provider_ByCompanyName()
        {
            Map = providers => from provider in providers
                select new
                {
                    provider.CompanyName
                };
        }
    }
}
