using System;
using Models;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class SaleConfirmationViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();

            CreateBasicData();

            _mainViewModel = new SaleConfirmationViewModel(_dataStorage);
        }

        [TearDown]
        public void CleanUpData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                session.Delete(session.Load<SaleConfirmation>(_documentId));
                session.Delete(session.Load<Customer>(_providerId));
                session.Delete(session.Load<Customer>(_customerId));
                session.SaveChanges();
            }
        }

        [Test]
        public void when_passing_a_document_id_it_retrieves_the_document()
        {
            _mainViewModel.Id = _documentId;

            Assert.That(_mainViewModel.SaleConfirmation, Is.Not.Null);
            Assert.That(_mainViewModel.SaleConfirmation.Id, Is.EqualTo(_documentId));
            Assert.That(_mainViewModel.SaleConfirmation.TruckLicensePlate, Is.EqualTo("AA000AA"));
            Assert.That(_mainViewModel.SaleConfirmation.TermsOfPayment, Is.EqualTo("bonifico 30 gg"));
        }

        [Test]
        public void when_passing_a_document_id_it_retrieves_the_customer_and_provider()
        {
            _mainViewModel.Id = _documentId;

            Assert.That(_mainViewModel.SaleConfirmation.Customer.Id, Is.EqualTo(_customerId));
            Assert.That(_mainViewModel.CustomerControlViewModel.Id, Is.EqualTo(_customerId));

            Assert.That(_mainViewModel.SaleConfirmation.Provider.Id, Is.EqualTo(_providerId));
            Assert.That(_mainViewModel.ProviderControlViewModel.Id, Is.EqualTo(_providerId));
        }

        [Test]
        public void when_passing_an_id_for_a_document_that_does_not_exist_the_document_is_blank()
        {
            _mainViewModel.Id = 100;

            Assert.That(_mainViewModel.SaleConfirmation, Is.Not.Null);
            Assert.That(_mainViewModel.SaleConfirmation.TruckLicensePlate, Is.Null);
        }

        [Test]
        public void when_adding_products_to_the_viewmodel_it_saves_the_list_of_products_sold()
        {
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel
            {
                Description = "Prodotto 1",
                Price = 12,
                Currency = "EUR"
            });
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel
            {
                Description = "Prodotto 2",
                Price = 13,
                Currency = "EUR"
            });

            _mainViewModel.SaveAll.Execute(null);

            SaleConfirmation document;
            using (var session = _dataStorage.CreateSession())
            {
                document = session.Load<SaleConfirmation>(_documentId);
            }
            Assert.That(document.ProductDetails, Is.Not.Null);
            Assert.That(document.ProductDetails.Count, Is.EqualTo(2));
        }

        private void CreateBasicData()
        {
            var document = new SaleConfirmation
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

            using (var session = _dataStorage.CreateSession())
            {
                session.Store(customer);
                session.Store(provider);
                session.Store(document);
                _documentId = document.Id;
                _customerId = customer.Id;
                _providerId = provider.Id;
                session.SaveChanges();
            }
        }

        private IDataStorage _dataStorage;
        private SaleConfirmationViewModel _mainViewModel;
        private int _documentId;
        private string _customerId;
        private string _providerId;
    }
}