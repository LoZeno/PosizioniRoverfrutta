using System;
using Models.Companies;
using Models.DocumentTypes;
using NUnit.Framework;

namespace QueryManager.Tests
{
    [TestFixture]
    public class SummaryAndInvoiceTest
    {
        private const string CompanyName = "Someone";
        private const string SecondCompanyName = "Someone Else";
        private RavenDataStorage _storage;

        [SetUp]
        public void SetUp()
        {
            _storage = new RavenDataStorage();
            _storage.Initialize();
        }

        [Test]
        public void WhenSavingSummaryAndInvoiceItGeneratesSequentialIds()
        {
            using (var session = _storage.CreateSession())
            {
                var invoice1 = new SummaryAndInvoice
                {
                    Customer = new Customer {CompanyName = CompanyName},
                    InvoiceDate = DateTime.Today
                };
                session.Store(invoice1);
                var invoice2 = new SummaryAndInvoice
                {
                    Customer = new Customer {CompanyName = SecondCompanyName},
                    InvoiceDate = DateTime.Today
                };
                session.Store(invoice2);
                session.SaveChanges();
            }
            SummaryAndInvoice invoiceNumberOne;
            SummaryAndInvoice invoiceNumberTwo;
            using (var newSession = _storage.CreateSession())
            {
                invoiceNumberOne = newSession.Load<SummaryAndInvoice>("summaryandinvoices/1");
                invoiceNumberTwo = newSession.Load<SummaryAndInvoice>("SummaryAndInvoices/2");
            }
            Assert.AreEqual(1, invoiceNumberOne.InvoiceNumber);
            Assert.AreEqual(2, invoiceNumberTwo.InvoiceNumber);
            Assert.AreEqual(CompanyName, invoiceNumberOne.Customer.CompanyName);
            Assert.AreEqual(SecondCompanyName, invoiceNumberTwo.Customer.CompanyName);
        }

        [Test]
        public void WhenSavingAnInvoiceWithAnArbitraryNumberThenStoresItWithTheCorrectId()
        {
            using (var session = _storage.CreateSession())
            {
                var invoceToStore = new SummaryAndInvoice
                {
                    InvoiceNumber = 23,
                    Customer = new Customer {CompanyName = CompanyName},
                    InvoiceDate = DateTime.Today
                };
                session.Store(invoceToStore);
                session.SaveChanges();
            }

            SummaryAndInvoice invoiceToLoad;
            using (var newSession = _storage.CreateSession())
            {
                invoiceToLoad = newSession.Load<SummaryAndInvoice>("SummaryAndInvoices/23");
            }
            Assert.AreEqual(23, invoiceToLoad.InvoiceNumber);
            Assert.AreEqual(CompanyName, invoiceToLoad.Customer.CompanyName);
        }

        [Test]
        public void WhenSavingAnInvoiceAfterAnotherWithAnArbitraryNumberThenTheNextOneHasASequentialId()
        {
            using (var session = _storage.CreateSession())
            {
                var invoice1 = new SummaryAndInvoice
                {
                    InvoiceNumber = 99,
                    Customer = new Customer { CompanyName = CompanyName },
                    InvoiceDate = DateTime.Today
                };
                session.Store(invoice1);
                session.SaveChanges();
            }
            using (var session = _storage.CreateSession())
            {
                var invoice2 = new SummaryAndInvoice
                {
                    Customer = new Customer { CompanyName = SecondCompanyName },
                    InvoiceDate = DateTime.Today
                };
                session.Store(invoice2);
                session.SaveChanges();
            }
            SummaryAndInvoice invoiceToLoad;
            using (var newSession = _storage.CreateSession())
            {
                invoiceToLoad = newSession.Load<SummaryAndInvoice>("SummaryAndInvoices/100");
            }
            Assert.AreEqual(100, invoiceToLoad.InvoiceNumber);
            Assert.AreEqual(SecondCompanyName, invoiceToLoad.Customer.CompanyName);
        }
    }
}
