using System.Linq;
using Models.Companies;
using Raven.Client.Documents.Indexes;

namespace QueryManager.Indexes;

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
