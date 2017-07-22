using Models.DocumentTypes;
using Raven.Client.Indexes;
using System;
using System.Globalization;
using System.Linq;

namespace QueryManager.Indexes
{
    public class ProductsStats_byProductIdAndWeek : AbstractIndexCreationTask<PriceConfirmation, ProductsStats_byProductIdAndWeek.ProductsDeconstruction>
    {
        public class ProductsDeconstruction
        {
            public int? ProductId { get; set; }
            public string Description { get; set; }
            public int Year { get; set; }
            public int Week { get; set; }
            public decimal NetWeight { get; set; }
        }

        public ProductsStats_byProductIdAndWeek()
        {
            Map = priceconfirmations => from priceconfirmation in priceconfirmations
                                        where priceconfirmation.ShippingDate.HasValue
                                        let weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(priceconfirmation.ShippingDate.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                                        from product in priceconfirmation.ProductDetails
                                        select new ProductsDeconstruction
                                        {
                                            ProductId = product.ProductId,
                                            Description = product.Description, 
                                            Year = priceconfirmation.ShippingDate.Value.Year, 
                                            Week = weekNumber,
                                            NetWeight = product.NetWeight
                                        };

            Reduce = results => from result in results
                                group result by new { productId = result.ProductId, year = result.Year, week = result.Week } into g
                                select new ProductsDeconstruction
                                {
                                    ProductId = g.Key.productId,
                                    Description = g.First().Description,
                                    Year = g.Key.year,
                                    Week = g.Key.week,
                                    NetWeight = g.Sum(p => p.NetWeight)
                                };
        }
    }
}
