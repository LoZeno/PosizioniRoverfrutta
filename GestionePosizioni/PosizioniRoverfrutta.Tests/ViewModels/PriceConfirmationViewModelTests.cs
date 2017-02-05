using System;
using System.Linq;
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
    public class PriceConfirmationViewModelTests
    {
        [TestFixtureSetUp]
        public void InitialSetup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _mockWindowManager = new Mock<IWindowManager>();
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(new DefaultValues
                {
                    Id = 1,
                    Vat = 20m,
                });
                session.SaveChanges();
            }
        }

        [SetUp]
        public void Setup()
        {
            _mainViewModel = new PriceConfirmationViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [TearDown]
        public void CleanUpData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var documents = session.Query<SaleConfirmation>().Select(x => x).ToList();
                foreach (var document in documents)
                {
                    session.Delete(document);
                }

                var loadingDocuments = session.Query<LoadingDocument>().Select(x => x).ToList();
                foreach (var document in loadingDocuments)
                {
                    session.Delete(document);
                }

                var priceConfirmations = session.Query<LoadingDocument>().Select(x => x).ToList();
                foreach (var document in priceConfirmations)
                {
                    session.Delete(document);
                }

                var customers = session.Query<Customer>().Select(x => x).ToList();
                foreach (var customer in customers)
                {
                    session.Delete(customer);
                }

                var products = session.Query<ProductDescription>().Select(x => x).ToList();
                foreach (var product in products)
                {
                    session.Delete(product);
                }
                session.SaveChanges();
            }
        }

        [Test]
        public void when_converting_a_loading_document_into_price_confirmation_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_disabled()
        {
            CreateLoadingDocument();
            _mainViewModel.Id = _documentId;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_searching_for_a_non_existing_document_all_buttons_are_disabled()
        {
            _mainViewModel.Id = 999999;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_loading_an_existing_document_the_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_property_is_edited_the_save_button_is_enabled_and_action_buttons_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "something";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_product_is_added_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel(new ProductDetails { Description = "Nuovo prodotto" }));
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_product_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(true);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails[0].GrossWeight = 100;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_customer_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.CompanyControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_provider_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProviderControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_transporter_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.TransporterControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_data_is_saved_then_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "qualche nota da aggiungere";
            _mainViewModel.SaveAll.Execute(null);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_document_is_reloaded_then_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "qualche nota da aggiungere";
            _mainViewModel.Reload.Execute(null);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        private void CreateBasicData(bool AddProduct)
        {
            var document = new PriceConfirmation
            {
                CustomerCommission = 3,
                DeliveryDate = DateTime.Now.AddDays(10),
                ProviderCommission = 3,
                ShippingDate = DateTime.Now,
                TermsOfPayment = "bonifico 30 gg",
                TruckLicensePlate = "AA000AA"
            };
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
            var provider = new Customer
            {
                CompanyName = "Fornitore",
                Address = "da un'altra parte",
                City = "MANTOVA",
                Country = "Italia",
                PostCode = "46100",
                StateOrProvince = "MN",
                VatCode = "partitaiva000000"
            };
            document.Customer = customer;
            document.Provider = provider;
            if (AddProduct)
            {
                document.ProductDetails.Add(new ProductDetails
                {
                    Currency = "EUR",
                    Description = "Un prodotto",
                });
            }

            using (var session = _dataStorage.CreateSession())
            {
                session.Store(customer);
                session.Store(provider);
                session.Store(document);
                if (AddProduct)
                {
                    session.Store(new ProductDescription
                    {
                        Description = document.ProductDetails[0].Description
                    });
                }
                session.SaveChanges();
            }
            _documentId = document.ProgressiveNumber;
            _customerId = customer.Id;
            _providerId = provider.Id;
        }

        private void CreateLoadingDocument()
        {
            var document = new LoadingDocument
            {
                CustomerCommission = 3,
                DeliveryDate = DateTime.Now.AddDays(10),
                ProviderCommission = 3,
                ShippingDate = DateTime.Now,
                TermsOfPayment = "bonifico 30 gg",
                TruckLicensePlate = "AA000AA"
            };
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
            var provider = new Customer
            {
                CompanyName = "Fornitore",
                Address = "da un'altra parte",
                City = "MANTOVA",
                Country = "Italia",
                PostCode = "46100",
                StateOrProvince = "MN",
                VatCode = "partitaiva000000"
            };
            document.Customer = customer;
            document.Provider = provider;
            document.ProductDetails.Add(new ProductDetails
            {
                Currency = "EUR",
                Description = "Un prodotto",
            });

            using (var session = _dataStorage.CreateSession())
            {
                session.Store(customer);
                session.Store(provider);
                session.Store(document);
                session.Store(new ProductDescription
                {
                    Description = document.ProductDetails[0].Description
                });

                session.SaveChanges();
            }
            _documentId = document.ProgressiveNumber;
            _customerId = customer.Id;
            _providerId = provider.Id;
        }

        private IDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;
        private PriceConfirmationViewModel _mainViewModel;
        private int _documentId;
        private string _customerId;
        private string _providerId;
    }
}
