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
    public class CustomerRepository : RepositoryBase<Customer>
    {
        public CustomerRepository(DocumentStore storage) : base(storage)
        {
        }

        public override string Add(Customer entity)
        {
            _session.Store(entity);
            return entity.Id;
        }
    }
}
