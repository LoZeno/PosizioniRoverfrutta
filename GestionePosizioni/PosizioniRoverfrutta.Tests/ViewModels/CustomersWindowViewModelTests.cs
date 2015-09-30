using System.Linq;
using Models.Companies;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class CustomersWindowViewModelTests
    {
        [TestFixtureSetUp]
        public void InitializeDataStorage()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _mockWindowManager = new Mock<IWindowManager>();
        }

        [SetUp]
        public void SetUp()
        {
            InsertInitialData();
            _viewModel = new CustomersWindowViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [TearDown]
        public void CleanUpDatabaase()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var customers = session.Query<Customer>().Select(x => x).ToList();
                foreach (var customer in customers)
                {
                    session.Delete(customer);
                }
                session.SaveChanges();
            }
        }

        [Test]
        public void When_initialized_it_should_loads_up_to_100_customers()
        {
            Assert.That(_viewModel.CustomersList.Count(), Is.EqualTo(100));
        }

        [TestCase("Customer", 10)]
        [TestCase("Provider", 15)]
        [TestCase("Position", 95)]
        [TestCase("filler", 80)]
        [TestCase("for", 80)]
        public void when_textbox_is_updated_it_should_narrow_the_search(string filter, int expectedTotal)
        {
            _viewModel.SearchBox = filter;
            Assert.That(_viewModel.CustomersList.Count(), Is.EqualTo(expectedTotal));
        }

        private void InsertInitialData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                for (int i = 0; i < 10; i++)
                {
                    var customerObject = new Customer()
                    {
                        CompanyName = "Customer Number " + i,
                    };
                    session.Store(customerObject);
                }
                for (int i = 0; i < 15; i++)
                {
                    var providerObject = new Customer()
                    {
                        CompanyName = "Provider Position " + i,
                    };
                    session.Store(providerObject);
                }

                for (int i = 0; i < 80; i++)
                {
                    var filler = new Customer()
                    {
                        CompanyName = "Filler for " + i + " position",
                    };
                    session.Store(filler);
                }
                session.SaveChanges();
            }
        }

        private IDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;
        private CustomersWindowViewModel _viewModel;
    }
}
