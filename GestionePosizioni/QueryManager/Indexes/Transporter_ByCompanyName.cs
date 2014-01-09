using Models;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
