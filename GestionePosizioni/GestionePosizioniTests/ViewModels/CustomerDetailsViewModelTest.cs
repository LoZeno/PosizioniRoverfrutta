using System.Collections.Generic;
using System.Linq;
using GestionePosizioni.ViewModels;
using Models;
using Moq;
using NUnit.Framework;
using QueryManager.Repositories;

namespace GestionePosizioniTests.ViewModels
{
    [TestFixture]
    public class CustomerDetailsViewModelTest
    {
        private ICustomerRepository customerRepo;
        private List<Customer> allCustomers;
        private Mock<ICustomerRepository> mockRepository;

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

            mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(x => x.FindById("customers/6")).Returns(GetSelectedCustomer("customers/6"));
            mockRepository.Setup(x => x.FindById("customers/1")).Returns(GetSelectedCustomer("customers/1"));
            mockRepository.Setup(x => x.FindByPartialName("Comp")).Returns(GetListOfCompanies("Comp"));
            mockRepository.Setup(x => x.Add(It.IsAny<Customer>())).Verifiable();
            mockRepository.Setup(x => x.Save()).Verifiable();
            

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

        [Test]
        public void Test_SettingNonExistantId_ReturnsNewCustomerObjectNotNull()
        {
            var viewModel = new CustomerDetailsViewModel(customerRepo);
            viewModel.Id = "customers/23";
            Assert.IsNotNull(viewModel.Company);
        }

        private void CheckSaveMethod()
        {
            mockRepository.Verify(m => m.Save());
        }

        [Test]
        public void Test_SaveModifiedCustomer()
        {
            var viewModel = new CustomerDetailsViewModel(customerRepo);
            viewModel.Id = "customers/1";
            viewModel.CompanyName = "TestCompany";
            viewModel.City = "fake city";
            viewModel.Country = "none";
            viewModel.Save.Execute(null);
            Assert.DoesNotThrow(CheckSaveMethod);
        }

        private void CheckAddMethod()
        {
            mockRepository.Verify(m => m.Add(It.IsAny<Customer>()));
        }

        [Test]
        public void Test_AddNewCustomer()
        {
            var viewModel = new CustomerDetailsViewModel(customerRepo);
            viewModel.CompanyName = "NewTestCompany";
            viewModel.City = "fake city";
            viewModel.Country = "none";
            viewModel.Save.Execute(null);
            Assert.DoesNotThrow(CheckAddMethod);
            Assert.DoesNotThrow(CheckSaveMethod);
        }
    }
}
