using System;
using System.Linq;
using System.Threading;
using Models.Companies;
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
    public class ListPositionsViewModelTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();

            _windowManager = new Mock<IWindowManager>();

            CreateBasicData();
            Thread.Sleep(10000);
        }

        [SetUp]
        public void Setup()
        {
            _mainViewModel = new ListPositionsViewModel(_dataStorage);
        }

        [Test]
        public void when_no_filter_is_selected_viewmodels_returns_the_last_100positions_ordered_by_id_descending()
        {
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(100));
        }

        [Test]
        public void when_setting_company_name_filter_it_returns_only_positions_with_that_customer()
        {
            _mainViewModel.CompanyName = "Customer 2";
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(50));
            Assert.That(_mainViewModel.PositionsList.All(e => e.CustomerName == "Customer 2"));
        }

        [Test]
        public void when_setting_company_name_filter_it_returns_only_positions_with_that_provider()
        {
            _mainViewModel.CompanyName = "Provider 1";
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(50));
            Assert.That(_mainViewModel.PositionsList.All(e => e.ProviderName == "Provider 1"));
        }

        [Test]
        public void when_setting_company_name_filter_with_a_name_used_both_as_provider_and_customer_it_returns_all_positions_with_that_company()
        {
            _mainViewModel.CompanyName = "Customer 1";
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(100));
            Assert.That(_mainViewModel.PositionsList.Count(e => e.CustomerName == "Customer 1"), Is.EqualTo(50));
            Assert.That(_mainViewModel.PositionsList.Count(e => e.ProviderName == "Customer 1"), Is.EqualTo(50));
        }

        [Test]
        public void when_setting_a_minimum_date_filter_viewmodels_returns_only_positions_more_recent_than_the_date()
        {
            _mainViewModel.FromDate = DateTime.Today.AddDays(170);
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(30));
        }

        [Test]
        public void when_setting_a_maximum_date_filter_viewmodels_returns_only_positions_older_than_the_date()
        {
            _mainViewModel.ToDate = DateTime.Today.AddDays(40);
            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(41));
        }

        [Test]
        public void when_a_position_has_an_associated_loading_document_the_HasLoadingDocument_property_is_true()
        {
            var loadingDocumentViewModel = new LoadingDocumentViewModel(_dataStorage, _windowManager.Object);
            loadingDocumentViewModel.Id = _mainViewModel.PositionsList[0].ProgressiveNumber;
            loadingDocumentViewModel.SaveAll.Execute(null);

            var loadingDocumentId = loadingDocumentViewModel.Id;

            _mainViewModel.FromDate = DateTime.Today.AddDays(180);

            Assert.That(_mainViewModel.PositionsList.Count(e => e.HasLoadingDocument), Is.EqualTo(1));
            Assert.That(_mainViewModel.PositionsList.Count(e => !e.HasLoadingDocument), Is.EqualTo(19));

            using (var session = _dataStorage.CreateSession())
            {
                var entity = session.Load<LoadingDocument>(loadingDocumentId);
                session.Delete<LoadingDocument>(entity);
                session.SaveChanges();
            }
        }

        [Test]
        public void when_a_position_has_an_associated_price_confirmation_the_HasPriceConfirmation_property_is_true()
        {
            var loadingDocumentViewModel = new LoadingDocumentViewModel(_dataStorage, _windowManager.Object);
            loadingDocumentViewModel.Id = _mainViewModel.PositionsList[0].ProgressiveNumber;
            loadingDocumentViewModel.SaveAll.Execute(null);

            var loadingDocumentId = loadingDocumentViewModel.Id;

            var priceConfirmationViewModel = new PriceConfirmationViewModel(_dataStorage, _windowManager.Object);
            priceConfirmationViewModel.Id = loadingDocumentViewModel.Id;
            priceConfirmationViewModel.SaveAll.Execute(null);

            var priceConfirmationId = priceConfirmationViewModel.Id;

            _mainViewModel.FromDate = DateTime.Today.AddDays(180);

            Assert.That(_mainViewModel.PositionsList.Count(e => e.HasLoadingDocument), Is.EqualTo(1));
            Assert.That(_mainViewModel.PositionsList.Count(e => e.HasPriceConfirmation), Is.EqualTo(1));
            Assert.That(_mainViewModel.PositionsList.Count(e => !e.HasLoadingDocument), Is.EqualTo(19));

            using (var session = _dataStorage.CreateSession())
            {
                var entity = session.Load<LoadingDocument>(loadingDocumentId);
                var entity2 = session.Load<PriceConfirmation>(priceConfirmationId);
                session.Delete<LoadingDocument>(entity);
                session.Delete<PriceConfirmation>(entity2);
                session.SaveChanges();
            }
        }

        [Test]
        public void when_no_row_is_selected_all_buttons_are_inactive()
        {
            _mainViewModel.HasFocus = true;
            _mainViewModel.SelectedPosition = null;

            Assert.That(_mainViewModel.OpenSaleConfirmationIsEnabled, Is.False);
            Assert.That(_mainViewModel.OpenLoadingDocumentIsEnabled, Is.False);
            Assert.That(_mainViewModel.OpenPriceConfirmationIsEnabled, Is.False);
        }

        [Test]
        public void when_a_row_is_selected_the_OpenSaleConfirmation_button_is_active()
        {
            _mainViewModel.HasFocus = true;
            _mainViewModel.SelectedPosition = _mainViewModel.PositionsList[0];

            Assert.That(_mainViewModel.OpenSaleConfirmationIsEnabled, Is.True);
            Assert.That(_mainViewModel.OpenLoadingDocumentIsEnabled, Is.False);
            Assert.That(_mainViewModel.OpenPriceConfirmationIsEnabled, Is.False);
        }

        [Test]
        public void when_a_row_with_loadingdocument_is_selected_the_OpenSaleConfirmation_and_OpenLoadingDocument_buttons_are_active()
        {
            _mainViewModel.HasFocus = true;
            var loadingDocumentViewModel = new LoadingDocumentViewModel(_dataStorage, _windowManager.Object);
            loadingDocumentViewModel.Id = _mainViewModel.PositionsList[0].ProgressiveNumber;
            loadingDocumentViewModel.SaveAll.Execute(null);

            var loadingDocumentId = loadingDocumentViewModel.Id;

            _mainViewModel.HasFocus = true;
            _mainViewModel.SelectedPosition = _mainViewModel.PositionsList[0];

            Assert.That(_mainViewModel.OpenSaleConfirmationIsEnabled, Is.True);
            Assert.That(_mainViewModel.OpenLoadingDocumentIsEnabled, Is.True);
            Assert.That(_mainViewModel.OpenPriceConfirmationIsEnabled, Is.False);

            using (var session = _dataStorage.CreateSession())
            {
                var entity = session.Load<LoadingDocument>(loadingDocumentId);
                session.Delete<LoadingDocument>(entity);
                session.SaveChanges();
            }
        }

        [Test]
        public void when_a_row_with_priceconfirmation_is_selected_the_OpenSaleConfirmation_and_OpenLoadingDocument_and_PriceConfirmation_buttons_are_active()
        {
            _mainViewModel.HasFocus = true;
            var loadingDocumentViewModel = new LoadingDocumentViewModel(_dataStorage, _windowManager.Object);
            loadingDocumentViewModel.Id = _mainViewModel.PositionsList[0].ProgressiveNumber;
            loadingDocumentViewModel.SaveAll.Execute(null);

            var loadingDocumentId = loadingDocumentViewModel.Id;

            var priceConfirmationViewModel = new PriceConfirmationViewModel(_dataStorage, _windowManager.Object);
            priceConfirmationViewModel.Id = loadingDocumentViewModel.Id;
            priceConfirmationViewModel.SaveAll.Execute(null);

            var priceConfirmationId = priceConfirmationViewModel.Id;

            _mainViewModel.HasFocus = true;
            _mainViewModel.SelectedPosition = _mainViewModel.PositionsList[0];

            Assert.That(_mainViewModel.OpenSaleConfirmationIsEnabled, Is.True);
            Assert.That(_mainViewModel.OpenLoadingDocumentIsEnabled, Is.True);
            Assert.That(_mainViewModel.OpenPriceConfirmationIsEnabled, Is.True);

            using (var session = _dataStorage.CreateSession())
            {
                var entity = session.Load<LoadingDocument>(loadingDocumentId);
                var entity2 = session.Load<PriceConfirmation>(priceConfirmationId);
                session.Delete<LoadingDocument>(entity);
                session.Delete<PriceConfirmation>(entity2);
                session.SaveChanges();
            }
        }

        [Test]
        public void should_skip_first_100_positions_if_next_is_clicked()
        {
            _mainViewModel.NextPage.Execute(null);

            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(100));
            Assert.That(_mainViewModel.PositionsList[0].ProgressiveNumber, Is.EqualTo(100));
        }

        [Test]
        public void should_return_back_100_positions_if_previous_is_clicked()
        {
            _mainViewModel.NextPage.Execute(null);
            _mainViewModel.PreviousPage.Execute(null);

            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(100));
            Assert.That(_mainViewModel.PositionsList[0].ProgressiveNumber, Is.EqualTo(200));
        }

        [Test]
        public void should_not_return_back_if_we_are_already_at_first_page()
        {
            _mainViewModel.PreviousPage.Execute(null);

            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(100));
            Assert.That(_mainViewModel.PositionsList[0].ProgressiveNumber, Is.EqualTo(200));
        }

        [Test]
        public void should_not_go_forward_if_we_are_already_at_last_page()
        {
            _mainViewModel.NextPage.Execute(null);
            _mainViewModel.NextPage.Execute(null);

            Assert.That(_mainViewModel.PositionsList.Count, Is.EqualTo(0));
        }

        private void CreateBasicData()
        {
            var customer1 = new Customer
            {
                CompanyName = "Customer 1"
            };

            var customer2 = new Customer
            {
                CompanyName = "Customer 2"
            };
            var customer3 = new Customer
            {
                CompanyName = "Customer 3"
            };

            var provider1 = new Customer
            {
                CompanyName = "Provider 1"
            };
            
            var provider2 = new Customer
            {
                CompanyName = "Provider 2"
            };
            
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(new DefaultValues{Id = 1, Vat = 22});
                session.Store(customer1);
                session.Store(customer2);
                session.Store(provider1);
                session.SaveChanges();
                for (int i = 0; i < 100; i++)
                {
                    var document = new SaleConfirmation
                    {
                        Customer = i%2 == 0 ? customer1 : customer2,
                        Provider = i%2 == 0 ? provider1 : customer1,
                        DocumentDate = DateTime.Today.AddDays(i),
                        ShippingDate = DateTime.Today.AddDays(i),
                    };
                    session.Store(document);
                }
                for (int i = 0; i < 100; i++)
                {
                    var document = new SaleConfirmation
                    {
                        Customer = customer3,
                        Provider = provider2,
                        DocumentDate = DateTime.Today.AddDays(100+i),
                        ShippingDate = DateTime.Today.AddDays(100+i),
                    };
                    session.Store(document);
                }
                session.SaveChanges();
            }
        }

        private RavenDataStorage _dataStorage;
        private ListPositionsViewModel _mainViewModel;
        private Mock<IWindowManager> _windowManager;
    }
}
