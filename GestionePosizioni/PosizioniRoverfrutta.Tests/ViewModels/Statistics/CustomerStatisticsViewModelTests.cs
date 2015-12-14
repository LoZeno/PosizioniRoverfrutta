using Models.Companies;
using Models.DocumentTypes;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels.Statistics;
using QueryManager;
using Raven.Client;

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

                var priceConfirmation = new PriceConfirmation
                {

                };

                session.Store(priceConfirmation);
                session.SaveChanges();

                _customerId = customer.Id;
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

        private IDataStorage _dataStorage;
        private string _customerId;

        const string customerName = "Customer";
        const string customerAddress = "Address";
        const string customerCity = "City";
        const string customerCountry = "Country";
        const string customerEmailAddress = "email@address.com";
        const string customerPostCode = "POSTCODE";
        const string customerState = "State";
        const string customerVatCode = "VatCode";
    }
}
