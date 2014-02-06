using System.ComponentModel;
using GestionePosizioni.ViewModels;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using QueryManager.Repositories;

namespace GestionePosizioniTests.ViewModels
{
    [TestFixture]
    public class MainViewModelTest
    {
        private ISaleConfirmationRepository _repository;
        private MainViewModel _viewModel;
        private Mock<ISaleConfirmationRepository> _mockRepository;
        private bool _totalsChanged;
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "TotalPallets")
            {
                _totalsChanged = true;
            }
        }

        [SetUp]
        public void Setup()
        {
            _totalsChanged = false;
            _mockRepository = new Mock<ISaleConfirmationRepository>();
            _repository = _mockRepository.Object;
            _mockRepository.Setup(x => x.FindById(121)).Returns(CreateSaleConfirmation(121, true));
            _mockRepository.Setup(x => x.FindById(122)).Returns(CreateSaleConfirmation(122, false));
            _mockRepository.Setup(x => x.Add(It.IsAny<SaleConfirmation>())).Verifiable();
            _mockRepository.Setup(x => x.Save()).Verifiable();

            _viewModel = new MainViewModel(new SaleConfirmation(), _repository);
        }

        private SaleConfirmation CreateSaleConfirmation(int documentId, bool addProduct)
        {
            var document = new SaleConfirmation
            {
                Id = documentId,
                Customer = new Customer
                {
                    Id = "customers/" + documentId,
                    CompanyName = "Customer " + documentId,
                    City = "City Name"
                },
                CustomerCommission = 5,
                DeliveryDate = new DateTime(2014, 01, 31),
                DeliveryEx = "Partenza",
                InvoiceDiscount = 0,
                Products = new List<ProductSold>(),
                Provider = new Customer
                {
                    Id = "providers/"+documentId,
                    CompanyName = "Provider "+documentId,
                    City = "City Name"
                },
                ProviderCommission = 0,
                Transporter = new Transporter
                {
                    Id = "transporters/"+documentId,
                    CompanyName = "Transporter "+documentId,
                    City = "City Name"
                }
            };

            if (addProduct)
            {
                document.Products.Add(new ProductSold
                {
                    Currency = "EUR",
                    GrossWeight = 200,
                    NetWeight = 190,
                    Packages = 10,
                    ProductDescription = "Product " + documentId,
                    ProductId = documentId
                });
            }

            return document;
        }

        [Test]
        public void Given_SaleConfirmationId_should_load_SaleConfirmation()
        {
            _viewModel.DocumentId = 121;

            Assert.IsNotNull(_viewModel.Customer);
            Assert.AreEqual("Customer 121",_viewModel.Customer.CompanyName);
            Assert.IsNotNull(_viewModel.Provider);
            Assert.AreEqual("Provider 121", _viewModel.Provider.CompanyName);
            Assert.AreEqual(1, _viewModel.Products.Count());
            Assert.AreEqual("Product 121", _viewModel.Products[0].ProductDescription);
            Assert.AreEqual(190, _viewModel.Products[0].NetWeight);
        }

        [Test]
        public void Given_multiple_products_sold_should_show_totals()
        {
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            _viewModel.DocumentId = 122;

            _viewModel.Products.Add(new ProductSold
            {
                Currency = "EUR",
                GrossWeight = 200,
                NetWeight = 190,
                Packages = 10,
                Pallets = 1,
                ProductDescription = "Product 1",
                ProductId = 1
            });
            _viewModel.Products.Add(new ProductSold
            {
                Currency = "EUR",
                GrossWeight = 400,
                NetWeight = 380,
                Packages = 20,
                Pallets = 2,
                ProductDescription = "Product 2",
                ProductId = 2
            });
            _viewModel.Products.Add(new ProductSold
            {
                Currency = "EUR",
                GrossWeight = 150,
                NetWeight = 149,
                Packages = 15,
                Pallets = 1,
                ProductDescription = "Product 3",
                ProductId = 3
            });

            Assert.AreEqual(4, _viewModel.TotalPallets);
            Assert.AreEqual(750, _viewModel.TotalGrossWeight);
            Assert.AreEqual(719, _viewModel.TotalNetWeight);
            Assert.AreEqual(45, _viewModel.TotalPackages);
            Assert.IsTrue(_totalsChanged);
        }

        [Test]
        public void Given_one_saleConfirmation_Saving_changes_productsSold()
        {
            var document = CreateSaleConfirmation(123, true);
            var mockProviderRepository = new Mock<ICustomerRepository>();
            mockProviderRepository.Setup(x => x.Add(It.IsAny<Customer>())).Verifiable();

            _viewModel = new MainViewModel(document, _repository, mockProviderRepository.Object);
            _viewModel.Products.RemoveAt(0);
            _viewModel.Products.Add(new ProductSold
            {
                Currency = "EUR",
                GrossWeight = 400,
                NetWeight = 380,
                Packages = 20,
                Pallets = 2,
                ProductDescription = "New Product",
                ProductId = 2
            });

            _viewModel.Save.Execute(null);
            Assert.AreEqual("New Product", document.Products.ElementAt(0).ProductDescription);
            _mockRepository.Verify();
        }

        [Test]
        public void Given_one_saleConfirmation_when_change_provider_and_save_provider_is_saved_in_providerRepository()
        {
            var mockProviderRepository = new Mock<ICustomerRepository>();
            mockProviderRepository.Setup(x => x.Add(It.IsAny<Customer>())).Verifiable();

            var document = CreateSaleConfirmation(123, true);
            _viewModel = new MainViewModel(document, _repository, mockProviderRepository.Object);

            _viewModel.Provider = new Customer
            {
                Id = "providers/125",
                CompanyName = "Test Provider",
                City = "City Name"
            };

            _viewModel.Save.Execute(null);

            Assert.AreEqual("Test Provider", _viewModel.Provider.CompanyName);
            mockProviderRepository.Verify();
        }
    }
}
