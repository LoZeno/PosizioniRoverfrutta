using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Raven.Client.Indexes;

namespace QueryManager.Indexes
{
    public class SaleConfirmation_ByCustomerId : AbstractIndexCreationTask<SaleConfirmation>
    {
        public SaleConfirmation_ByCustomerId()
        {
            Map = saleconfirmations => from saleconfirmation in saleconfirmations
                select new
                {
                    Customer_Id = saleconfirmation.Customer.Id
                };
        }
    }
}
