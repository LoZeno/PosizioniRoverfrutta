using System.Collections.Generic;
using Models;
using Models.DocumentTypes;
using Raven.Client;

namespace QueryManager.Repositories
{
    public interface ISaleConfirmationRepository
    {
        int Add(SaleConfirmation entity);
        SaleConfirmation FindById(int documentId);
        void Delete(SaleConfirmation entity);
        void Save();
        IEnumerable<SaleConfirmation> FindByCustomerId(string customerId);
        IDocumentSession Session { get; set; }
    }
}