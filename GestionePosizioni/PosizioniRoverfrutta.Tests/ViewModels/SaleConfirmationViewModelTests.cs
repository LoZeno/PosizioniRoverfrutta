﻿using System;
using Models.Companies;
using Models.DocumentTypes;
using Models.Entities;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using Raven.Client.Linq;
using System.Linq;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class SaleConfirmationViewModelTests
    {
        [TestFixtureSetUp]
        public void MainSetup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _mockWindowManager = new Mock<IWindowManager>();
        }

        [SetUp]
        public void Setup()
        {
            _mainViewModel = new SaleConfirmationViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [TearDown]
        public void CleanUpData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var documents = session.Query<SaleConfirmation>().Select(x => x).ToList();
                foreach (var document in documents)
                {
                    session.Delete(document);
                }

                var customers = session.Query<Customer>().Select(x => x).ToList();
                foreach (var customer in customers)
                {
                    session.Delete(customer);
                }

                var products = session.Query<ProductDescription>().Select(x => x).ToList();
                foreach (var product in products)
                {
                    session.Delete(product);
                }
                session.SaveChanges();
            }
        }

        [Test]
        public void when_passing_a_document_id_it_retrieves_the_document()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;

            Assert.That(_mainViewModel.SaleConfirmation, Is.Not.Null);
            Assert.That(_mainViewModel.SaleConfirmation.ProgressiveNumber, Is.EqualTo(_documentId));
            Assert.That(_mainViewModel.SaleConfirmation.TruckLicensePlate, Is.EqualTo("AA000AA"));
            Assert.That(_mainViewModel.SaleConfirmation.TermsOfPayment, Is.EqualTo("bonifico 30 gg"));
        }

        [Test]
        public void when_passing_a_document_id_it_retrieves_the_customer_and_provider()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;

            Assert.That(_mainViewModel.SaleConfirmation.Customer.Id, Is.EqualTo(_customerId));
            Assert.That(_mainViewModel.CompanyControlViewModel.Id, Is.EqualTo(_customerId));

            Assert.That(_mainViewModel.SaleConfirmation.Provider.Id, Is.EqualTo(_providerId));
            Assert.That(_mainViewModel.ProviderControlViewModel.Id, Is.EqualTo(_providerId));
        }

        [Test]
        public void when_passing_an_id_for_a_document_that_does_not_exist_the_document_is_blank()
        {
            CreateBasicData(false);
            _mainViewModel.Id = 100;

            Assert.That(_mainViewModel.SaleConfirmation, Is.Not.Null);
            Assert.That(_mainViewModel.SaleConfirmation.TruckLicensePlate, Is.Null);
        }

        [Test]
        public void when_adding_products_to_the_viewmodel_it_saves_the_list_of_products_sold()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel
            {
                Description = "Prodotto 1",
                Price = 12,
                Currency = "EUR"
            });
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel
            {
                Description = "Prodotto 2",
                Price = 13,
                Currency = "EUR"
            });

            _mainViewModel.SaveAll.Execute(null);

            SaleConfirmation document;
            using (var session = _dataStorage.CreateSession())
            {
                document = session.Load<SaleConfirmation>(_documentId);
            }
            Assert.That(document.ProductDetails, Is.Not.Null);
            Assert.That(document.ProductDetails.Count, Is.EqualTo(2));
        }

        [Test]
        public void when_creating_a_new_position_the_save_button_and_action_buttons_and_reload_button_are_initially_disabled()
        {
            CreateBasicData(false);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }
        
        [Test]
        public void when_loading_a_document_the_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_property_is_edited_the_save_button_is_enabled_and_action_buttons_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "something";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_product_is_added_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails.Add(new ProductRowViewModel(new ProductDetails{Description = "Nuovo prodotto"}));
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_product_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(true);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProductDetails[0].GrossWeight = 100;
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_customer_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.CompanyControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_provider_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.ProviderControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_transporter_is_edited_the_save_button_is_enabled_and_action_buttons_are_disabled_and_reload_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.TransporterControlViewModel.City = "Parma";
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.True);
        }

        [Test]
        public void when_data_is_saved_then_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "qualche nota da aggiungere";
            _mainViewModel.SaveAll.Execute(null);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_document_is_reloaded_then_save_button_is_disabled_and_action_buttons_are_enabled_and_reload_button_is_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Notes = "qualche nota da aggiungere";
            _mainViewModel.Reload.Execute(null);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.False);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.True);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_document_is_cloned_then_the_save_button_is_enabled_and_action_buttons_and_reload_buttons_are_disabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.Clone.Execute(null);
            Assert.That(_mainViewModel.SaveButtonEnabled, Is.True);
            Assert.That(_mainViewModel.ActionButtonsEnabled, Is.False);
            Assert.That(_mainViewModel.ReloadButtonEnabled, Is.False);
        }

        [Test]
        public void when_selecting_open_attachments_it_opens_windows_explorer_to_the_correct_folder()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            _mainViewModel.OpenAttachments.Execute(null);

            _mockWindowManager.Verify(manager => manager.OpenAttachmentWindow(_documentId));
        }

        [Test]
        public void when_creating_a_new_position_the_attachments_button_is_disabled()
        {
            CreateBasicData(false);
            Assert.That(_mainViewModel.OpenAttachmentsButtonEnabled, Is.False);
        }

        [Test]
        public void when_saving_a_new_position_the_attachments_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.SaveAll.Execute(null);
            Assert.That(_mainViewModel.OpenAttachmentsButtonEnabled, Is.True);
        }

        [Test]
        public void when_loading_an_existing_position_the_attachments_button_is_enabled()
        {
            CreateBasicData(false);
            _mainViewModel.Id = _documentId;
            Assert.That(_mainViewModel.OpenAttachmentsButtonEnabled, Is.True);
        }

        private void CreateBasicData(bool AddProduct)
        {
            var document = new SaleConfirmation
            {
                CustomerCommission = 3,
                DeliveryDate = DateTime.Now.AddDays(10),
                ProviderCommission = 3,
                ShippingDate = DateTime.Now,
                TermsOfPayment = "bonifico 30 gg",
                TruckLicensePlate = "AA000AA"
            };
            var customer = new Customer
            {
                CompanyName = "Cliente",
                Address = "da qualche parte",
                City = "MANTOVA",
                Country = "Italia",
                PostCode = "46100",
                StateOrProvince = "MN",
                VatCode = "partitaiva000000"
            };
            var provider = new Customer
            {
                CompanyName = "Fornitore",
                Address = "da un'altra parte",
                City = "MANTOVA",
                Country = "Italia",
                PostCode = "46100",
                StateOrProvince = "MN",
                VatCode = "partitaiva000000"
            };
            document.Customer = customer;
            document.Provider = provider;
            if (AddProduct)
            {
                document.ProductDetails.Add(new ProductDetails
                {
                    Currency = "EUR",
                    Description = "Un prodotto",
                });
            }

            using (var session = _dataStorage.CreateSession())
            {
                session.Store(customer);
                session.Store(provider);
                session.Store(document);
                if (AddProduct)
                {
                    session.Store(new ProductDescription
                    {
                        Description = document.ProductDetails[0].Description
                    });
                }
                session.SaveChanges();
            }
            _documentId = document.ProgressiveNumber;
            _customerId = customer.Id;
            _providerId = provider.Id;
        }

        private IDataStorage _dataStorage;
        private SaleConfirmationViewModel _mainViewModel;
        private int _documentId;
        private string _customerId;
        private string _providerId;
        private Mock<IWindowManager> _mockWindowManager;
    }
}