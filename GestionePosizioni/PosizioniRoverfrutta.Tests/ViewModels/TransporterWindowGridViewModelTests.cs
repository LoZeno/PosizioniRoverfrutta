using Models.Companies;
using Models.DocumentTypes;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class TransporterWindowGridViewModelTests
    {
        [TestFixtureSetUp]
        public void InitializeDataStorage()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _dataStorage.DocumentStore.Conventions.ShouldSaveChangesForceAggressiveCacheCheck = true;
            _mockWindowManager = new Mock<IWindowManager>();
        }

        [SetUp]
        public void SetUp()
        {
            InsertInitialData();
            _viewModel = new TransporterWindowGridViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [TearDown]
        public void CleanUpDatabaase()
        {
            using (var session = _dataStorage.CreateSession())
            {
                var transporters = session.Query<Transporter>().ToList();
                foreach (var transporter in transporters)
                {
                    session.Delete(transporter);
                }
                var salesCon = session.Query<SaleConfirmation>().ToList();
                foreach (var saleConfirmation in salesCon)
                {
                    session.Delete(saleConfirmation);
                }
                var loadDoc = session.Query<LoadingDocument>().ToList();
                foreach (var saleConfirmation in loadDoc)
                {
                    session.Delete(saleConfirmation);
                }
                var priceConf = session.Query<PriceConfirmation>().ToList();
                foreach (var saleConfirmation in priceConf)
                {
                    session.Delete(saleConfirmation);
                }
                session.SaveChanges();
            }
        }

        [Test]
        public void When_initialized_it_should_loads_up_to_100_transporters_in_alphabetical_order()
        {
            Assert.That(_viewModel.TransportersList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.TransportersList[0].CompanyName, Is.EqualTo("C-Transporter Number 0"));
            Assert.That(_viewModel.TransportersList[99].CompanyName, Is.EqualTo("P-Transporter Position 4"));
        }

        [TestCase("C-Transporter", 10)]
        [TestCase("P-transporter", 15)]
        [TestCase("Position", 95)]
        [TestCase("filler", 80)]
        [TestCase("for", 80)]
        public void when_textbox_is_updated_it_should_narrow_the_search(string filter, int expectedTotal)
        {
            _viewModel.SearchBox = filter;
            Assert.That(_viewModel.TransportersList.Count(), Is.EqualTo(expectedTotal));
        }

        [Test]
        public void when_clicking_next_page_it_should_load_the_next_100_transporters()
        {
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.TransportersList.Count, Is.EqualTo(5));
            Assert.That(_viewModel.TransportersList[0].CompanyName, Is.EqualTo("P-Transporter Position 5"));
        }

        [Test]
        public void when_clicking_next_page_if_there_is_no_excess_data_it_will_not_change_page()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.TransportersList.Count, Is.EqualTo(5));
            Assert.That(_viewModel.TransportersList[0].CompanyName, Is.EqualTo("P-Transporter Position 5"));
        }

        [Test]
        public void when_clicking_previous_page_if_there_is_not_a_previous_page_of_data_it_will_not_change_page()
        {
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.TransportersList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.TransportersList[0].CompanyName, Is.EqualTo("C-Transporter Number 0"));
            Assert.That(_viewModel.TransportersList[99].CompanyName, Is.EqualTo("P-Transporter Position 4"));
        }

        [Test]
        public void when_clicking_previous_page_after_next_page_it_returns_to_the_actual_previous_page_of_data()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.TransportersList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.TransportersList[0].CompanyName, Is.EqualTo("C-Transporter Number 0"));
            Assert.That(_viewModel.TransportersList[99].CompanyName, Is.EqualTo("P-Transporter Position 4"));
        }

        [Test]
        public void when_window_is_active_then_it_refreshes_the_data()
        {
            string newTransporterPositionedAsFirst = "AAA New Transporter";
            var newTransporter = new Transporter
            {
                CompanyName = newTransporterPositionedAsFirst
            };
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(newTransporter);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.TransportersList.Any(c => c.CompanyName.Equals(newTransporterPositionedAsFirst)), Is.True);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_transporter_is_deleted_elsewhere_then_the_selected_transporter_fields_are_emptied()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<Transporter>(selectedTransporterId);
                session.Delete(itemToDelete);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.CompanyName, Is.Null);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_transporter_has_changed_then_the_field_is_updated()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToEdit = session.Load<Transporter>(selectedTransporterId);
                itemToEdit.Address = "A New Address";
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.Address, Is.EqualTo("A New Address"));
        }

        [Test]
        public void when_editing_a_company_and_clicking_save_it_refreshes_the_data_in_the_grid_and_empties_the_editing_controls()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            _viewModel.CompanyName = "AAA New Company Name";

            _viewModel.Save.Execute(null);
            Assert.That(_viewModel.TransportersList.Any(c => c.CompanyName.Equals("AAA New Company Name")), Is.True);
            Assert.That(string.IsNullOrWhiteSpace(_viewModel.CompanyName), Is.True);
        }

        [Test]
        public void when_the_transporter_is_not_selected_the_delete_button_is_disabled()
        {
            _viewModel.LoadSelectedTransporter(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_transporter_with_no_associated_positions_is_selected_the_delete_button_is_enabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_transporter_with_at_least_one_associated_saleconfirmation_as_transporter_is_selected_the_delete_button_is_disabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var transporter = session.Load<Transporter>(selectedTransporterId);
                var sc = new SaleConfirmation
                {
                    Transporter = transporter
                };
                session.Store(sc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_Transporter_with_at_least_one_associated_loadingDocument_as_Transporter_is_selected_the_delete_button_is_disabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var Transporter = session.Load<Transporter>(selectedTransporterId);
                var ld = new LoadingDocument
                {
                    Transporter = Transporter
                };
                session.Store(ld);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }


        [Test]
        public void when_a_Transporter_with_at_least_one_associated_priceconfirmation_as_Transporter_is_selected_the_delete_button_is_disabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            using (var session = _dataStorage.CreateSession())
            {
                var Transporter = session.Load<Transporter>(selectedTransporterId);
                var pc = new PriceConfirmation
                {
                    Transporter = Transporter
                };
                session.Store(pc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_no_Transporter_is_selected_the_save_button_is_disabled()
        {
            _viewModel.LoadSelectedTransporter(null);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_Transporter_is_selected_but_not_edited_the_save_button_is_disabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_Transporter_is_edited_the_save_button_is_enabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            _viewModel.Address = "Some New Address";
            Assert.That(_viewModel.SaveButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_new_Transporter_is_created_the_save_button_and_delete_button_are_disabled()
        {
            _viewModel.CreateNew.Execute(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_Transporter_does_not_have_a_CompanyName_then_the_save_button_is_disabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            _viewModel.Address = "something";
            _viewModel.CompanyName = string.Empty;
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_Transporter_is_new_the_delete_button_is_disabled_even_when_it_has_a_company_name()
        {
            _viewModel.CreateNew.Execute(null);
            _viewModel.CompanyName = "Somethingsomething";
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_delete_button_is_pressed_it_deletes_the_Transporter_and_refresh_the_grid()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            _viewModel.DeleteTransporter.Execute(null);
            Assert.That(_viewModel.TransportersList.Any(x => x.Id.Equals(selectedTransporterId)), Is.False);
        }

        [Test]
        public void when_selected_Transporter_is_null_edit_controls_are_disabled()
        {
            _viewModel.LoadSelectedTransporter(null);
            Assert.That(_viewModel.EditControlsEnabled, Is.False);
        }

        [Test]
        public void when_selected_Transporter_is_new_Transporter_edit_controls_are_enabled()
        {
            _viewModel.CreateNew.Execute(null);
            Assert.That(_viewModel.EditControlsEnabled, Is.True);
        }

        [Test]
        public void when_selected_Transporter_is_existing_Transporter_edit_controls_are_enabled()
        {
            var selectedTransporterId = _viewModel.TransportersList[0].Id;
            _viewModel.LoadSelectedTransporter(selectedTransporterId);
            Assert.That(_viewModel.EditControlsEnabled, Is.True);
        }

        private void InsertInitialData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                for (int i = 0; i < 10; i++)
                {
                    var TransporterObject = new Transporter()
                    {
                        CompanyName = "C-Transporter Number " + i,
                    };
                    session.Store(TransporterObject);
                }
                for (int i = 0; i < 15; i++)
                {
                    var providerObject = new Transporter()
                    {
                        CompanyName = "P-Transporter Position " + i,
                    };
                    session.Store(providerObject);
                }

                for (int i = 0; i < 80; i++)
                {
                    var filler = new Transporter()
                    {
                        CompanyName = "Filler for " + i + " position",
                    };
                    session.Store(filler);
                }
                session.SaveChanges();
            }
        }

        private IDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;
        private TransporterWindowGridViewModel _viewModel;
    }
}
