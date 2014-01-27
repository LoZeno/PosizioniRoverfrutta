using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public interface IProviderRepository
    {
        string Add(Provider entity);
        IEnumerable<Provider> FindByPartialName(string partialName);
        Provider FindById(string custID);
        void Delete(Provider entity);
        void Save();
        IDocumentSession DatabaseSession { get; set; }
    }
}