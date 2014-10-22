using Models;
using Models.Companies;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class CustomerControlViewModelTests
    {

        [SetUp]
        public void SetUp()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _viewModel = new CompanyControlViewModel<Customer>(_dataStorage);

            CreateBasicData();
        }

        [TearDown]
        public void CleanUpData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Delete(session.Load<Customer>(_customerId));
                session.SaveChanges();
            }
        }

        [Test]
        public void when_initialized_returns_a_new_customer_objects()
        {
            Assert.That(_viewModel.Company, Is.Not.Null);
        }

        [Test]
        public void when_writing_a_new_company_name_it_creates_a_new_company()
        {
            _viewModel.CompanyName = "Nuovo Cliente";
            Assert.That(_viewModel.Company.Id, Is.EqualTo(null));
            Assert.That(_viewModel.Company.CompanyName, Is.EqualTo("Nuovo Cliente"));
            Assert.That(_viewModel.Company.VatCode, Is.EqualTo(null));
        }

        [Test]
        public void when_writing_an_existing_company_name_it_loads_the_first_match()
        {
            _viewModel.CompanyName = "Cliente";

            Assert.That(_viewModel.Id, Is.EqualTo(_customerId));
            Assert.That(_viewModel.CompanyName, Is.EqualTo("Cliente"));
            Assert.That(_viewModel.VatCode, Is.EqualTo("partitaiva000000"));
        }

        private void CreateBasicData()
        {
            var customer = new Customer
            {
                CompanyName = "Cliente",
                Address = "da qualche parte",
                City = "MANTOVA",
                Country = "Italia",
                PostCode = "46100",
                StateOrProvince = "MN",
                VatCode = "partitaiva000000"
            };

            using (var session = _dataStorage.CreateSession())
            {
                session.Store(customer);
                _customerId = customer.Id;
                session.SaveChanges();
            }
        }

        private CompanyControlViewModel<Customer> _viewModel;

        private IDataStorage _dataStorage;
        private string _customerId;
    }
}
