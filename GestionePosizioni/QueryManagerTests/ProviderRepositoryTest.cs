using System.Linq;
using Models;
using NUnit.Framework;
using QueryManager;
using QueryManager.Repositories;

namespace QueryManagerTests
{
    [TestFixture]
    public class ProviderRepositoryTest
    {
        private RavenDataStorage storage;
        private ProviderRepository repository;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            storage = new RavenDataStorage();
            storage.Initialize();
        }

        [Test]
        public void Test_CreateNewProvider_ShouldReturnCreatedProviderId()
        {
            var provider = new Provider();
            string custID;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                custID = repository.Add(provider);
                mySession.SaveChanges();
            }
            Assert.That(custID.StartsWith("providers/"));
        }

        [Test]
        public void Test_SearchProviderById_shouldReturnProviderObject()
        {
            var provider = new Provider
            {
                CompanyName = "Company 1",
                Country = "Italy"
            };
            var provider2 = new Provider
            {
                CompanyName = "Company 2",
                Country = "UK"
            };
            string id;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                repository.Add(provider);
                id = repository.Add(provider2);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                var retrievedData = repository.FindById(id);
                Assert.AreEqual("Company 2", retrievedData.CompanyName);
            }
        }

        [Test]
        public void Test_UpdateProvider()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var provider = new Provider
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new ProviderRepository(mySession);
                custId = repository.Add(provider);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                var providerToUpdate = repository.FindById(custId);
                providerToUpdate.CompanyName = "New Company Name";
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                var providerToCheck = repository.FindById(custId);
                Assert.AreEqual("New Company Name", providerToCheck.CompanyName);
            }
        }

        [Test]
        public void Test_DeleteProvider()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var provider = new Provider
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new ProviderRepository(mySession);
                custId = repository.Add(provider);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                var providerToDelete = repository.FindById(custId);
                repository.Delete(providerToDelete);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mySession);
                var providerToCheck = repository.FindById(custId);
                Assert.IsNull(providerToCheck);
            }
        }

        [Test]
        public void Test_FindProvidersByName()
        {
            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mysession);
                for (int i = 0; i < 10; i++)
                {
                    var provider = new Provider
                    {
                        CompanyName = i % 2 == 0 ? "Search Company " + i : "Find This " + i,
                        Country = "Italy"
                    };
                    repository.Add(provider);
                }
                mysession.SaveChanges();
            }

            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new ProviderRepository(mysession);
                var results = repository.FindByPartialName("This");
                Assert.AreEqual(5, results.Count());
            }
        }
    }
}
