using System.Linq;
using System.Threading;
using Models.Companies;
using Models.DocumentTypes;
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
            _dataStorage.DocumentStore.Conventions.ShouldSaveChangesForceAggressiveCacheCheck = true;
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
                var customers = session.Query<Customer>().ToList();
                foreach (var customer in customers)
                {
                    session.Delete(customer);
                }
                var salesCon = session.Query<SaleConfirmation>().ToList();
                foreach (var saleConfirmation in salesCon)
                {
                    session.Delete(saleConfirmation);
                }
                var loadDoc = session.Query<LoadingDocument>().ToList();
                foreach (var saleConfirmation in loadDoc)
                {
                    session.Delete(saleConfirmation);
                }
                var priceConf = session.Query<PriceConfirmation>().ToList();
                foreach (var saleConfirmation in priceConf)
                {
                    session.Delete(saleConfirmation);
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

        [Test]
        public void when_window_is_active_then_it_refreshes_the_data()
        {
            string newCustomerPositionedAsFirst = "AAA New Customer";
            var newCustomer = new Customer
            {
                CompanyName = newCustomerPositionedAsFirst
            };
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(newCustomer);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.CustomersList.Any(c => c.CompanyName.Equals(newCustomerPositionedAsFirst)), Is.True);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_customer_is_deleted_elsewhere_then_the_selected_customer_fields_are_emptied()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<Customer>(selectedCustomerId);
                session.Delete(itemToDelete);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.CompanyName, Is.Null);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_customer_has_changed_then_the_field_is_updated()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToEdit = session.Load<Customer>(selectedCustomerId);
                itemToEdit.Address = "A New Address";
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.Address, Is.EqualTo("A New Address"));
        }

        [Test]
        public void when_editing_a_company_and_clicking_save_it_refreshes_the_data_in_the_grid()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            _viewModel.CompanyName = "AAA New Company Name";

            _viewModel.Save.Execute(null);
            Assert.That(_viewModel.CustomersList.Any(c => c.CompanyName.Equals("AAA New Company Name")), Is.True);
        }

        [Test]
        public void when_the_customer_is_not_selected_the_delete_button_is_disabled()
        {
            _viewModel.LoadSelectedCustomer(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_no_associated_positions_is_selected_the_delete_button_is_enabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_saleconfirmation_as_customer_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var sc = new SaleConfirmation
                {
                    Customer = customer
                };
                session.Store(sc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_saleconfirmation_as_provider_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var sc = new SaleConfirmation
                {
                    Provider = customer
                };
                session.Store(sc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_loadingDocument_as_customer_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var ld = new LoadingDocument
                {
                    Customer = customer
                };
                session.Store(ld);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_loadingDocument_as_provider_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var ld = new LoadingDocument
                {
                    Provider = customer
                };
                session.Store(ld);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_priceconfirmation_as_customer_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var pc = new PriceConfirmation
                {
                    Customer = customer
                };
                session.Store(pc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_with_at_least_one_associated_priceconfirmation_as_provider_is_selected_the_delete_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var customer = session.Load<Customer>(selectedCustomerId);
                var pc = new PriceConfirmation
                {
                    Provider = customer
                };
                session.Store(pc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_no_customer_is_selected_the_save_button_is_disabled()
        {
            _viewModel.LoadSelectedCustomer(null);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_is_selected_but_not_edited_the_save_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_customer_is_edited_the_save_button_is_enabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            _viewModel.Address = "Some New Address";
            Assert.That(_viewModel.SaveButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_new_customer_is_created_the_save_button_and_delete_button_are_disabled()
        {
            _viewModel.CreateNew.Execute(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_customer_does_not_have_a_CompanyName_then_the_save_button_is_disabled()
        {
            var selectedCustomerId = _viewModel.CustomersList[0].Id;
            _viewModel.LoadSelectedCustomer(selectedCustomerId);
            _viewModel.Address = "something";
            _viewModel.CompanyName = string.Empty;
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_customer_is_new_the_delete_button_is_disabled_even_when_it_has_a_company_name()
        {
            _viewModel.CreateNew.Execute(null);
            _viewModel.CompanyName = "Somethingsomething";
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
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
