using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using NUnit.Framework;
using QueryManager;
using QueryManager.Repositories;

namespace QueryManagerTests
{
    [TestFixture]
    public class SaleConfirmationRepositoryTest
    {
        private RavenDataStorage storage;
        private SaleConfirmationRepository repository;

        [TestFixtureSetUp]
        public void Setup()
        {
            storage = new RavenDataStorage();
            storage.Initialize();
        }

        [Test]
        public void Test_CreateNewSaleConfirmation_ShouldReturnSaleConfirmationId()
        {
            var saleConfirmation = new SaleConfirmation();
            int scId;
            using (var session = storage.DocumentStore.OpenSession())
            {
                repository = new SaleConfirmationRepository(session);
                scId = repository.Add(saleConfirmation);
                session.SaveChanges();
            }
            Assert.That(scId != 0);
        }

        [Test]
        public void Test_SearchSaleConfirmationById_ShouldReturnSaleConfirmationDocument()
        {
            var sc1 = new SaleConfirmation
            {
                ShippingDate = new DateTime(2012, 3, 1),
                TruckLicensePlate = "ABC123"
            };
            var sc2 = new SaleConfirmation
            {
                ShippingDate = new DateTime(2012, 4, 1),
                TruckLicensePlate = "DEF456"
            };
            int scId;
            using (var session = storage.DocumentStore.OpenSession())
            {
                repository = new SaleConfirmationRepository(session);
                repository.Add(sc1);
                scId = repository.Add(sc2);
                session.SaveChanges();
            }

            using (var session = storage.DocumentStore.OpenSession())
            {
                repository = new SaleConfirmationRepository(session);
                var document = repository.FindById(scId);
                Assert.AreEqual("DEF456", document.TruckLicensePlate);
            }
        }

        [Test]
        public void Test_AddSaleConfirmationWithProducts_ReturnsFullListOfProducts()
        {
            var sc1 = new SaleConfirmation
            {
                ShippingDate = new DateTime(2014, 1, 10),
                TruckLicensePlate = "123ABC",
                Products = new List<ProductSold>
                {
                    new ProductSold
                    {
                        ProductId = 1,
                        Currency = "EUR",
                        GrossWeight = 100,
                        NetWeight = 90,
                        ProductDescription = "Apples"
                    },
                    new ProductSold
                    {
                        ProductId = 2,
                        Currency = "EUR",
                        GrossWeight = 120,
                        NetWeight = 95,
                        ProductDescription = "Pears"
                    }
                }
            };
            int scId;
            using (var session = storage.DocumentStore.OpenSession())
            {
                repository = new SaleConfirmationRepository(session);
                scId = repository.Add(sc1);
                session.SaveChanges();
            }

            using (var session = storage.DocumentStore.OpenSession())
            {
                repository = new SaleConfirmationRepository(session);
                var document = repository.FindById(scId);
                Assert.AreEqual(2, document.Products.Count);
                Assert.AreEqual("Apples", document.Products[0].ProductDescription);
                Assert.AreEqual("Pears", document.Products[1].ProductDescription);
            }
        }
    }
}
