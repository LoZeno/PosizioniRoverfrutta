using Models.DocumentTypes;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryManager.Indexes
{
    public class PriceConfirmation_byCustomerIdAndProviderIdAndShippingDate : AbstractIndexCreationTask<PriceConfirmation>
    {
        public PriceConfirmation_byCustomerIdAndProviderIdAndShippingDate()
        {
            Map = priceconfirmations => from priceconfirmation in priceconfirmations
                                       select new
                                       {
                                           Customer_Id = priceconfirmation.Customer.Id,
                                           Provider_Id = priceconfirmation.Provider.Id,
                                           ShippingDate = priceconfirmation.ShippingDate
                                       };
        }
    }
}
