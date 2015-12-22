using Models.Companies;
using Models.DocumentTypes;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels.Statistics;
using QueryManager;
using QueryManager.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PosizioniRoverfrutta.Tests.ViewModels.Statistics
{
    [TestFixture]
    public class CustomerStatisticsViewModelTests
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
                    CompanyName = customerName,
                    Address = customerAddress,
                    City = customerCity,
                    Country = customerCountry,
                    EmailAddress = customerEmailAddress,
                    PostCode = customerPostCode,
                    StateOrProvince = customerState,
                    VatCode = customerVatCode
                };
                session.Store(customer);
                var provider = new Customer
                {
                    CompanyName = providerName,
                    Address = providerAddress,
                    City = providerCity,
                    Country = providerCountry,
                    EmailAddress = providerEmailAddress,
                    PostCode = providerPostCode,
                    StateOrProvince = providerState,
                    VatCode = providerVatCode
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
                        ProductId = i + 10,
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

                _customerId = customer.Id;
                _providerId = provider.Id;
            }
        }

        [Test]
        public void when_loading_the_viewmodel_it_loads_the_customer()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);

            Assert.That(viewModel.CustomerName, Is.EqualTo(customerName));
            Assert.That(viewModel.Address, Is.EqualTo(customerAddress));
            Assert.That(viewModel.City, Is.EqualTo(customerCity));
            Assert.That(viewModel.Country, Is.EqualTo(customerCountry));
            Assert.That(viewModel.EmailAddress, Is.EqualTo(customerEmailAddress));
            Assert.That(viewModel.PostCode, Is.EqualTo(customerPostCode));
            Assert.That(viewModel.StateOrProvince, Is.EqualTo(customerState));
            Assert.That(viewModel.VatCode, Is.EqualTo(customerVatCode));
        }

        [Test]
        public void when_loading_the_viewModel_it_defaults_to_Customer()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);

            Assert.That(viewModel.CustomerOrProvider, Is.EqualTo(StatisticsMode.Customer));
        }

        [Test]
        public void when_no_date_filter_is_set_the_list_of_products_is_empty()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [Test]
        public void when_the_from_date_filter_only_is_set_the_list_of_products_is_empty()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.FromDate = DateTime.Today;
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [Test]
        public void when_the_to_date_filter_only_is_set_the_list_of_products_is_empty()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.ToDate = DateTime.Today;
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [TestCase(1,2)]
        [TestCase(2,3)]
        [TestCase(1,3)]
        [TestCase(1, 10)]
        [TestCase(3, 10)]
        [TestCase(0, 29)]
        public void when_all_filters_are_set_for_customer_the_list_of_products_contains_data_within_the_dates_selected_included(int startDay, int endDay)
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.FromDate = DateTime.Today.AddDays(startDay);
            viewModel.ToDate = DateTime.Today.AddDays(endDay);

            var firstSeed = 0;
            var secondSeed = 0;
            var numberOfDays = endDay - startDay + 1;
            var firstExpectedTotalAmount = 0;
            var secondExpectedTotalAmount = 0;
            for (int i = startDay; i < endDay + 1; i++)
            {
                firstSeed += i + 1;
                secondSeed += i + 2;
                var firstAmount = (i + 1) * (i + 1) * 100;
                firstExpectedTotalAmount += firstAmount;
                var secondAmount = (i + 2) * (i + 2) * 100;
                secondExpectedTotalAmount += secondAmount;
            }
            Assert.That(viewModel.ProductStatisticsRows.Count, Is.EqualTo(2 + numberOfDays));
            //first product grouped
            Assert.That(viewModel.ProductStatisticsRows[0].ProductId, Is.EqualTo(1));
            Assert.That(viewModel.ProductStatisticsRows[0].NetWeight, Is.EqualTo(firstSeed));
            Assert.That(viewModel.ProductStatisticsRows[0].AveragePrice, Is.EqualTo(firstSeed * 100 / numberOfDays));
            Assert.That(viewModel.ProductStatisticsRows[0].Description, Is.EqualTo("Product number 1"));
            Assert.That(viewModel.ProductStatisticsRows[0].TotalAmount, Is.EqualTo(firstExpectedTotalAmount));
            //second product grouped
            Assert.That(viewModel.ProductStatisticsRows[1].ProductId, Is.EqualTo(2));
            Assert.That(viewModel.ProductStatisticsRows[1].NetWeight, Is.EqualTo(secondSeed));
            Assert.That(viewModel.ProductStatisticsRows[1].AveragePrice, Is.EqualTo(secondSeed * 100 / numberOfDays));                     
            Assert.That(viewModel.ProductStatisticsRows[1].Description, Is.EqualTo("Product number 2"));                                     
            Assert.That(viewModel.ProductStatisticsRows[1].TotalAmount, Is.EqualTo(secondExpectedTotalAmount));
        }

        [Test]
        public void when_fromDate_is_reset_to_null_the_list_of_products_is_emptied()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.FromDate = DateTime.Today.AddDays(1);
            viewModel.ToDate = DateTime.Today.AddDays(2);

            viewModel.FromDate = null;
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [Test]
        public void when_toDate_is_reset_to_null_the_list_of_products_is_emptied()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.FromDate = DateTime.Today.AddDays(1);
            viewModel.ToDate = DateTime.Today.AddDays(2);

            viewModel.ToDate = null;
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [Test]
        public void when_switching_to_provider_mode_and_the_company_has_no_data_as_provider_it_returns_empty_list()
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _customerId);
            viewModel.FromDate = DateTime.Today.AddDays(1);
            viewModel.ToDate = DateTime.Today.AddDays(2);

            viewModel.CustomerOrProvider = StatisticsMode.Provider;
            Assert.That(viewModel.ProductStatisticsRows.Any(), Is.False);
        }

        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(1, 3)]
        [TestCase(1, 10)]
        [TestCase(3, 10)]
        [TestCase(0, 29)]
        public void when_all_filters_are_set_for_provider_the_list_of_products_contains_data_within_the_dates_selected_included(int startDay, int endDay)
        {
            var viewModel = new CustomerStatisticsViewModel(_dataStorage, _providerId);
            viewModel.CustomerOrProvider = StatisticsMode.Provider;
            viewModel.FromDate = DateTime.Today.AddDays(startDay);
            viewModel.ToDate = DateTime.Today.AddDays(endDay);

            var firstSeed = 0;
            var secondSeed = 0;
            var numberOfDays = endDay - startDay + 1;
            var firstExpectedTotalAmount = 0;
            var secondExpectedTotalAmount = 0;
            for (int i = startDay; i < endDay + 1; i++)
            {
                firstSeed += i + 1;
                secondSeed += i + 2;
                var firstAmount = (i + 1) * (i + 1) * 100;
                firstExpectedTotalAmount += firstAmount;
                var secondAmount = (i + 2) * (i + 2) * 100;
                secondExpectedTotalAmount += secondAmount;
            }
            Assert.That(viewModel.ProductStatisticsRows.Count, Is.EqualTo(2 + numberOfDays));
            //first product grouped
            Assert.That(viewModel.ProductStatisticsRows[0].ProductId, Is.EqualTo(1));
            Assert.That(viewModel.ProductStatisticsRows[0].NetWeight, Is.EqualTo(firstSeed));
            Assert.That(viewModel.ProductStatisticsRows[0].AveragePrice, Is.EqualTo(firstSeed * 100 / numberOfDays));
            Assert.That(viewModel.ProductStatisticsRows[0].Description, Is.EqualTo("Product number 1"));
            Assert.That(viewModel.ProductStatisticsRows[0].TotalAmount, Is.EqualTo(firstExpectedTotalAmount));
            //second product grouped
            Assert.That(viewModel.ProductStatisticsRows[1].ProductId, Is.EqualTo(2));
            Assert.That(viewModel.ProductStatisticsRows[1].NetWeight, Is.EqualTo(secondSeed));
            Assert.That(viewModel.ProductStatisticsRows[1].AveragePrice, Is.EqualTo(secondSeed * 100 / numberOfDays));
            Assert.That(viewModel.ProductStatisticsRows[1].Description, Is.EqualTo("Product number 2"));
            Assert.That(viewModel.ProductStatisticsRows[1].TotalAmount, Is.EqualTo(secondExpectedTotalAmount));
        }

        private IDataStorage _dataStorage;
        private string _customerId;
        private string _providerId;

        const string customerName = "Customer";
        const string customerAddress = "Address";
        const string customerCity = "City";
        const string customerCountry = "Country";
        const string customerEmailAddress = "email@address.com";
        const string customerPostCode = "POSTCODE";
        const string customerState = "State";
        const string customerVatCode = "VatCode";
        private string providerName = "Provider";
        private string providerAddress = "Other Address";
        private string providerCity = "Other City";
        private string providerCountry = "Other Country";
        private string providerEmailAddress = "another.email@address.com";
        private string providerPostCode = "P0STC0D3";
        private string providerState ="another State";
        private string providerVatCode = "Other VatCode";
    }
}
