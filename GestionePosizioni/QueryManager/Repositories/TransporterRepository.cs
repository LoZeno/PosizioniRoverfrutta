using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public class TransporterRepository : CompanyRepositoryBase<Transporter>, ITransporterRepository
    {
        public TransporterRepository(IDocumentSession session) : base(session)
        {
        }
    }
}
