using Models;
using Raven.Client.Indexes;
using System.Linq;

namespace QueryManager.Indexes
{
    public class Transporter_ByCompanyName : AbstractIndexCreationTask<Transporter>
    {
        public Transporter_ByCompanyName()
        {
            Map = transporters => from transporter in transporters
                select new
                {
                    transporter.CompanyName
                };
        }
    }
}
