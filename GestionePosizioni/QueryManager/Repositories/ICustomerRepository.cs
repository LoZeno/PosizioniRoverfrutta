using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public interface ICustomerRepository
    {
        string Add(Customer entity);
        IEnumerable<Customer> FindByPartialName(string partialName);
        Customer FindById(string custId);
        void Delete(Customer entity);
        void Save();
        IDocumentSession DatabaseSession { get; set; }
    }
}