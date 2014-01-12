using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionePosizioni.ViewModels;
using Models;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using QueryManager.Repositories;

namespace GestionePosizioniTests.ViewModels
{
    [TestFixture]
    public class CustomerDetailsViewModelTest
    {
        private ICustomerRepository customerRepo;
        private List<Customer> allCustomers;

        [TestFixtureSetUp]
        public void TestSetup()
        {
            allCustomers = new List<Customer>();
            for (int i = 0; i < 5; i++)
            {
                allCustomers.Add(new Customer
                {
                    Id = "customers/"+i,
                    CompanyName = "Company " + i
                });
            }
            for (int i = 5; i < 10; i++)
            {
                allCustomers.Add(new Customer
                {
                    Id = "customers/" + i,
                    CompanyName = "Customer " + i
                });
            }

            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(x => x.FindById("customers/6")).Returns(GetSelectedCustomer("customers/6"));
            mockRepository.Setup(x => x.FindByPartialName("Comp")).Returns(GetListOfCompanies("Comp"));


            customerRepo = mockRepository.Object;
        }

        private IEnumerable<Customer> GetListOfCompanies(string partialName)
        {
            return allCustomers.Where(c => c.CompanyName.Contains(partialName));
        }

        private Customer GetSelectedCustomer(string customerId)
        {
            return allCustomers.FirstOrDefault(c => c.Id == customerId);
        }
        [Test]
        public void Test_ChangeCompanyName_LoadsCompaniesWithSameSubstring()
        {
            var viewModel = new CustomerDetailsViewModel(customerRepo);
            viewModel.CompanyName = "Comp";
            Assert.AreEqual(5, viewModel.Companies.Count);
        }

        [Test]
        public void Test_SettingID_LoadsCustomer()
        {
            var viewModel = new CustomerDetailsViewModel(customerRepo);
            viewModel.Id = "customers/6";
            Assert.AreEqual("Customer 6", viewModel.CompanyName);
        }
    }
}
