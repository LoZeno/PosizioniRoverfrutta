using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using NUnit.Framework;
using QueryManager;
using Raven.Client;

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
    }
}
