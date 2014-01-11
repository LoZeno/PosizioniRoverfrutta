using System.Linq;
using Models;
using NUnit.Framework;
using QueryManager;
using QueryManager.Repositories;

namespace QueryManagerTests
{
    [TestFixture]
    public class TransporterRepositoryTest
    {
        private RavenDataStorage storage;
        private TransporterRepository repository;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            storage = new RavenDataStorage();
            storage.Initialize();
        }

        [Test]
        public void Test_CreateNewTransporter_ShouldReturnCreatedTransporterId()
        {
            var transporter = new Transporter();
            string custID;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                custID = repository.Add(transporter);
                mySession.SaveChanges();
            }
            Assert.That(custID.StartsWith("transporters/"));
        }

        [Test]
        public void Test_SearchTransporterById_shouldReturnTransporterObject()
        {
            var transporter = new Transporter
            {
                CompanyName = "Company 1",
                Country = "Italy"
            };
            var transporter2 = new Transporter
            {
                CompanyName = "Company 2",
                Country = "UK"
            };
            string id;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                repository.Add(transporter);
                id = repository.Add(transporter2);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                var retrievedData = repository.FindById(id);
                Assert.AreEqual("Company 2", retrievedData.CompanyName);
            }
        }

        [Test]
        public void Test_UpdateTransporter()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var transporter = new Transporter
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new TransporterRepository(mySession);
                custId = repository.Add(transporter);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                var transporterToUpdate = repository.FindById(custId);
                transporterToUpdate.CompanyName = "New Company Name";
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                var transporterToCheck = repository.FindById(custId);
                Assert.AreEqual("New Company Name", transporterToCheck.CompanyName);
            }
        }

        [Test]
        public void Test_DeleteTransporter()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var transporter = new Transporter
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new TransporterRepository(mySession);
                custId = repository.Add(transporter);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                var transporterToDelete = repository.FindById(custId);
                repository.Delete(transporterToDelete);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mySession);
                var transporterToCheck = repository.FindById(custId);
                Assert.IsNull(transporterToCheck);
            }
        }

        [Test]
        public void Test_FindTransportersByName()
        {
            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mysession);
                for (int i = 0; i < 10; i++)
                {
                    var transporter = new Transporter
                    {
                        CompanyName = i % 2 == 0 ? "Search Company " + i : "Find This " + i,
                        Country = "Italy"
                    };
                    repository.Add(transporter);
                }
                mysession.SaveChanges();
            }

            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new TransporterRepository(mysession);
                var results = repository.FindByPartialName("This");
                Assert.AreEqual(5, results.Count());
            }
        }
    }
}
