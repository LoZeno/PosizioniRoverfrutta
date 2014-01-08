using System.Linq;
using Models;
using NUnit.Framework;
using QueryManager;
using QueryManager.Repositories;
using Raven.Client.Document;

namespace QueryManagerTests
{
    [TestFixture]
    public class CustomerRepositoryTest
    {
        private RavenDataStorage storage;
        private CustomerRepository repository;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            storage = new RavenDataStorage();
            storage.Initialize();
        }

        [Test]
        public void Test_CreateNewCustomer_ShouldReturnCreatedCustomerId()
        {
            var customer = new Customer();
            string custID;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                custID = repository.Add(customer);
                mySession.SaveChanges();
            }
            Assert.That(custID.StartsWith("customers/"));
        }

        [Test]
        public void Test_SearchCustomerById_shouldReturnCustomerObject()
        {
            var customer = new Customer
            {
                CompanyName = "Company 1",
                Country = "Italy"
            };
            var customer2 = new Customer
            {
                CompanyName = "Company 2",
                Country = "UK"
            };
            string id;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                repository.Add(customer);
                id = repository.Add(customer2);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                var retrievedData = repository.FindById(id);
                Assert.AreEqual("Company 2",retrievedData.CompanyName);
            }
        }

        [Test]
        public void Test_UpdateCustomer()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var customer = new Customer
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new CustomerRepository(mySession);
                custId = repository.Add(customer);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                var customerToUpdate = repository.FindById(custId);
                customerToUpdate.CompanyName = "New Company Name";
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                var customerToCheck = repository.FindById(custId);
                Assert.AreEqual("New Company Name", customerToCheck.CompanyName);
            }
        }

        [Test]
        public void Test_DeleteCustomer()
        {
            string custId;
            using (var mySession = storage.DocumentStore.OpenSession())
            {
                var customer = new Customer
                {
                    CompanyName = "Company Name",
                    Country = "Italy"
                };
                repository = new CustomerRepository(mySession);
                custId = repository.Add(customer);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                var customerToDelete = repository.FindById(custId);
                repository.Delete(customerToDelete);
                mySession.SaveChanges();
            }

            using (var mySession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mySession);
                var customerToCheck = repository.FindById(custId);
                Assert.IsNull(customerToCheck);
            }
        }

        [Test]
        public void Test_FindCustomersByName()
        {
            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mysession);
                for (int i = 0; i < 10; i++)
                {
                    var customer = new Customer
                    {
                        CompanyName = i % 2 == 0 ? "Search Company " + i : "Find This " + i,
                        Country = "Italy"
                    };
                    repository.Add(customer);
                }
                mysession.SaveChanges();
            }

            using (var mysession = storage.DocumentStore.OpenSession())
            {
                repository = new CustomerRepository(mysession);
                var results = repository.FindByPartialName("This");
                Assert.AreEqual(5, results.Count());
            }
        }
    }
}
