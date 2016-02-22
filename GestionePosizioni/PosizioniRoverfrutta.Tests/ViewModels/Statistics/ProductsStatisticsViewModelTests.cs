using System;
using System.Collections.Generic;
using Models.Companies;
using Models.DocumentTypes;
using NUnit.Framework;
using QueryManager;
using Raven.Client.Document;

namespace PosizioniRoverfrutta.Tests.ViewModels.Statistics
{
    [TestFixture]
    public class ProductsStatisticsViewModelTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _dataStorage.DocumentStore.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;

            using (var session = _dataStorage.CreateSession())
            {
                var customer = new Customer
                {
                    CompanyName = "Customer",
                };
                session.Store(customer);
                var provider = new Customer
                {
                    CompanyName = "Provider",
                };
                session.Store(provider);

                for (int i = 0; i < 30; i++)
                {
                    var productsSold = new List<ProductDetails>();
                    productsSold.Add(new ProductDetails
                    {
                        ProductId = 1,
                        Description = "Product number 1",
                        Pallets = i + 1,
                        Packages = i + 1,
                        GrossWeight = i + 1,
                        NetWeight = i + 1,
                        PriceParameter = i + 1,
                        Price = (i + 1) * 100
                    });

                    productsSold.Add(new ProductDetails
                    {
                        ProductId = 2,
                        Description = "Product number 2",
                        Pallets = i + 2,
                        Packages = i + 2,
                        GrossWeight = i + 2,
                        NetWeight = i + 2,
                        PriceParameter = i + 2,
                        Price = (i + 2) * 100
                    });

                    productsSold.Add(new ProductDetails
                    {
                        ProductId = i + 90,
                        Description = "Product number i",
                        Pallets = i + 1,
                        Packages = i + 1,
                        GrossWeight = i + 1,
                        NetWeight = i + 1,
                        PriceParameter = i + 1,
                        Price = (i + 1) * 100
                    });

                    var shippingDate = DateTime.Today.AddDays(i);
                    var priceConfirmation = new PriceConfirmation
                    {
                        Customer = customer,
                        Provider = provider,
                        ShippingDate = shippingDate,
                        ProductDetails = productsSold
                    };

                    session.Store(priceConfirmation);
                }
                session.SaveChanges();
            }
        }

        private IDataStorage _dataStorage;
    }
}
