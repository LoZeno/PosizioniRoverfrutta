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
    public class CustomersWindowGridViewModelTests
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
            _viewModel = new CustomersWindowGridViewModel(_dataStorage, _mockWindowManager.Object);
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
        public void When_initialized_it_should_loads_up_to_100_customers_in_alphabetical_order()
        {
            Assert.That(_viewModel.CustomersList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.CustomersList[0].CompanyName, Is.EqualTo("Customer Number 0"));
            Assert.That(_viewModel.CustomersList[99].CompanyName, Is.EqualTo("Provider Position 4"));
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

        [Test]
        public void when_clicking_next_page_it_should_load_the_next_100_customers()
        {
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.CustomersList.Count, Is.EqualTo(5));
            Assert.That(_viewModel.CustomersList[0].CompanyName, Is.EqualTo("Provider Position 5"));
        }

        [Test]
        public void when_clicking_next_page_if_there_is_no_excess_data_it_will_not_change_page()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.CustomersList.Count, Is.EqualTo(5));
            Assert.That(_viewModel.CustomersList[0].CompanyName, Is.EqualTo("Provider Position 5"));
        }

        [Test]
        public void when_clicking_previous_page_if_there_is_not_a_previous_page_of_data_it_will_not_change_page()
        {
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.CustomersList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.CustomersList[0].CompanyName, Is.EqualTo("Customer Number 0"));
            Assert.That(_viewModel.CustomersList[99].CompanyName, Is.EqualTo("Provider Position 4"));
        }

        [Test]
        public void when_clicking_previous_page_after_next_page_it_returns_to_the_actual_previous_page_of_data()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.CustomersList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.CustomersList[0].CompanyName, Is.EqualTo("Customer Number 0"));
            Assert.That(_viewModel.CustomersList[99].CompanyName, Is.EqualTo("Provider Position 4"));
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
        private CustomersWindowGridViewModel _viewModel;
    }
}
