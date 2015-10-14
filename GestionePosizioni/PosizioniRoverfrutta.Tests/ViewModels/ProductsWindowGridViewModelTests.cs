using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class ProductsWindowGridViewModelTests
    {
        [TestFixtureSetUp]
        public void InitializeDataStorage()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _dataStorage.DocumentStore.Conventions.ShouldSaveChangesForceAggressiveCacheCheck = true;
            _mockWindowManager = new Mock<IWindowManager>();
        }

        [SetUp]
        public void SetUp()
        {
            InsertInitialData();
            _viewModel = new ProductsWindowGridViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [TearDown]
        public void CleanUpDatabaase()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var products = session.Query<ProductDescription>();
                foreach (var customer in products)
                {
                    session.Delete(customer);
                }
                var salesCon = session.Query<SaleConfirmation>();
                foreach (var saleConfirmation in salesCon)
                {
                    session.Delete(saleConfirmation);
                }
                var loadDoc = session.Query<LoadingDocument>();
                foreach (var saleConfirmation in loadDoc)
                {
                    session.Delete(saleConfirmation);
                }
                var priceConf = session.Query<PriceConfirmation>();
                foreach (var saleConfirmation in priceConf)
                {
                    session.Delete(saleConfirmation);
                }
                session.SaveChanges();
            }
        }

        [Test]
        public void when_initialized_it_should_load_up_to_100_products_in_alphabetical_order()
        {
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        [TestCase("99", 2)]
        [TestCase("thing", 100)]
        [TestCase("for", 0)]
        public void when_textbox_is_updated_it_should_narrow_the_search(string filter, int expectedTotal)
        {
            _viewModel.SearchBox = filter;
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(expectedTotal));
        }

        [Test]
        public void when_clicking_next_page_it_should_load_the_next_100_customers()
        {
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Product Number 0"));
        }

        [Test]
        public void when_clicking_next_page_if_there_is_no_excess_data_it_will_not_change_page()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Product Number 0"));
        }

        [Test]
        public void when_clicking_previous_page_if_there_is_not_a_previous_page_of_data_it_will_not_change_page()
        {
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        [Test]
        public void when_clicking_previous_page_after_next_page_it_returns_to_the_actual_previous_page_of_data()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        private void InsertInitialData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                for (int i = 99; i >= 0; i--)
                {
                    var newProduct = new ProductDescription
                    {
                        Description = "Product Number " + i,
                    };
                    session.Store(newProduct);
                }
                for (int i = 0; i < 100; i++)
                {
                    var newProduct = new ProductDescription
                    {
                        Description = "Another thing " + i,
                    };
                    session.Store(newProduct);
                }
                session.SaveChanges();
            }
        }

        private RavenDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;
        private ProductsWindowGridViewModel _viewModel;
    }
}
