using System;
using System.Collections.Generic;
using System.Linq;
using Models.Companies;
using Models.DocumentTypes;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class SummaryAndInvoiceViewModelTests
    {
        private const string ProviderName = "sameProvider";
        private const string CompanyName1 = "Company1";
        private const string CompanyName2 = "Company2";
        private RavenDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _mockWindowManager = new Mock<IWindowManager>();

            StoreInitialData();

        }

        [Test]
        public void ShowsPartialSummaryOfPayableAmountByCompanyName()
        {
            var viewModel = new SummaryAndInvoiceViewModel(_dataStorage, _mockWindowManager.Object);

            viewModel.CustomerName = ProviderName;
            viewModel.StartDate = DateTime.Today.AddDays(-1);
            viewModel.EndDate = DateTime.Today.AddDays(1);

            Assert.AreEqual(2, viewModel.PartialsByCompanyName.Count);
        }

        [Test]
        public void ShowsCorrectCommissionAmountForEachCompanyName()
        {
            var viewModel = new SummaryAndInvoiceViewModel(_dataStorage, _mockWindowManager.Object);

            viewModel.CustomerName = ProviderName;
            viewModel.StartDate = DateTime.Today.AddDays(-1);
            viewModel.EndDate = DateTime.Today.AddDays(1);

            Assert.AreEqual(40, viewModel.PartialsByCompanyName.First(row => row.CompanyName.Equals(CompanyName1)).Total);
            Assert.AreEqual(60, viewModel.PartialsByCompanyName.First(row => row.CompanyName.Equals(CompanyName2)).Total);
        }

        private void StoreInitialData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var customer1 = new Customer {CompanyName = CompanyName1};
                session.Store(customer1);
                var customer2 = new Customer {CompanyName = CompanyName2};
                session.Store(customer2);
                var provider = new Customer {CompanyName = ProviderName};
                session.Store(provider);
                session.SaveChanges();

                var priceConfirmationList = new List<PriceConfirmation>();
                for (int i = 0; i < 10; i++)
                {
                    var priceConfirmation = new PriceConfirmation();
                    priceConfirmation.Customer = i%3 == 0 ? customer1 : customer2;
                    priceConfirmation.Provider = provider;
                    priceConfirmation.ProviderCommission = 10;
                    priceConfirmation.ShippingDate = DateTime.Today;
                    priceConfirmation.TaxableAmount = 100;
                    priceConfirmationList.Add(priceConfirmation);
                }

                priceConfirmationList.ForEach(document => session.Store(document));
                session.SaveChanges();
            }
        }
    }
}
