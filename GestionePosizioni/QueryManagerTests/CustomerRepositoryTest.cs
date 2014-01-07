using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using NUnit.Framework;
using QueryManager;

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
            using (repository = new CustomerRepository(storage.DocumentStore))
            {
                custID = repository.Add(customer);
                repository.Save();
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
            using (repository = new CustomerRepository(storage.DocumentStore))
            {
                repository.Add(customer);
                id = repository.Add(customer2);
                repository.Save();
            }

            using (repository = new CustomerRepository(storage.DocumentStore))
            {
                var retrievedData = repository.FindById(id);
                Assert.AreEqual("Company 2",retrievedData.CompanyName);
            }
        }
    }
}
