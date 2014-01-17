using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public interface ICustomerRepository
    {
        string Add(Customer entity);
        IEnumerable<Customer> FindByPartialName(string partialName);
        IEnumerable<string> GetCompaniesForListBox(string partialName);
        Customer FindById(string custID);
        void Delete(Customer entity);
        void Save();
        IDocumentSession DatabaseSession { get; set; }
    }
}