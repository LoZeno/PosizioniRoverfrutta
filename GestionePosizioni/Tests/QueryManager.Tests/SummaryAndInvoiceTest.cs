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
            string id1;
            string id2;
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
                id1 = invoice1.Id;
                id2 = invoice2.Id;
            }
            SummaryAndInvoice invoiceNumberOne;
            SummaryAndInvoice invoiceNumberTwo;
            using (var newSession = _storage.CreateSession())
            {
                invoiceNumberOne = newSession.Load<SummaryAndInvoice>(id1);
                invoiceNumberTwo = newSession.Load<SummaryAndInvoice>(id2);
            }
            Assert.That(invoiceNumberOne.InvoiceNumber, Is.EqualTo(1));
            Assert.That(invoiceNumberTwo.InvoiceNumber, Is.EqualTo(2));
            Assert.That(invoiceNumberOne.Customer.CompanyName, Is.EqualTo(CompanyName));
            Assert.That(invoiceNumberTwo.Customer.CompanyName, Is.EqualTo(SecondCompanyName));
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
            Assert.That(invoiceToLoad.InvoiceNumber, Is.EqualTo(23));
            Assert.That(invoiceToLoad.Customer.CompanyName, Is.EqualTo(CompanyName));
        }

        [Test]
        [Ignore("Sequential IDs after arbitrary numbers are not guaranteed")]
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
            Assert.That(invoiceToLoad.InvoiceNumber, Is.EqualTo(100));
            Assert.That(invoiceToLoad.Customer.CompanyName, Is.EqualTo(SecondCompanyName));
        }
    }
}
