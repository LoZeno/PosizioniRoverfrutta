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
    }
}
