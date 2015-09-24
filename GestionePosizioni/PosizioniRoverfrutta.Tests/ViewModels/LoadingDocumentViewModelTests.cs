using System;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class LoadingDocumentViewModelTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();

            CreateBasicData(false);

            _mainViewModel = new LoadingDocumentViewModel(_dataStorage, null);
        }

        private void CreateBasicData(bool AddProduct)
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

        private IDataStorage _dataStorage;
        private LoadingDocumentViewModel _mainViewModel;
        private int _documentId;
        private string _customerId;
        private string _providerId;
    }
}
