using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class SaleConfirmationRepository
    {
        private IDocumentSession _session;

        public SaleConfirmationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public int Add(SaleConfirmation entity)
        {
            _session.Store(entity);
            return entity.Id;
        }

        public SaleConfirmation FindById(int documentId)
        {
            return _session.Load<SaleConfirmation>(documentId);
        }

        public void Delete(SaleConfirmation entity)
        {
            _session.Delete(entity);
        }

        public void Save()
        {
            _session.SaveChanges();
        }

        public IEnumerable<SaleConfirmation> FindByCustomerId(string customerId)
        {
            return _session.Query<SaleConfirmation>("SaleConfirmation/ByCustomerId")
                .Where(sc => sc.Customer.Id.Equals(customerId));
        }
    }
}
