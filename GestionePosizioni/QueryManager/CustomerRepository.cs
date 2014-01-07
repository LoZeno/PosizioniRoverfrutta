using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Storage.Esent;

namespace QueryManager
{
    public class CustomerRepository : IDocumentRepository<Customer>
    {
        private IDocumentSession _session;

        public CustomerRepository(DocumentStore storage)
        {
            _session = storage.OpenSession();
        }

        public string Add(Customer customer)
        {
            _session.Store(customer);
            return customer.Id;
        }

        public Customer FindById(string custId)
        {
            return _session.Load<Customer>(custId);
        }

        public void Save()
        {
            _session.SaveChanges();
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
