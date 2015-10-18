using System.Linq;
using Models.DocumentTypes;
using Models.Entities;
using Moq;
using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class ProductsWindowGridViewModelTests
    {
        [TestFixtureSetUp]
        public void InitializeDataStorage()
        {
            _mockWindowManager = new Mock<IWindowManager>();
        }

        [SetUp]
        public void SetUp()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();
            _dataStorage.DocumentStore.Conventions.ShouldSaveChangesForceAggressiveCacheCheck = true;
            InsertInitialData();
            _viewModel = new ProductsWindowGridViewModel(_dataStorage, _mockWindowManager.Object);
        }

        [Test]
        public void when_initialized_it_should_load_up_to_100_products_in_alphabetical_order()
        {
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        [TestCase("99", 2)]
        [TestCase("thing", 100)]
        [TestCase("for", 0)]
        public void when_textbox_is_updated_it_should_narrow_the_search(string filter, int expectedTotal)
        {
            _viewModel.SearchBox = filter;
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(expectedTotal));
        }

        [Test]
        public void when_clicking_next_page_it_should_load_the_next_100_customers()
        {
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Product Number 0"));
        }

        [Test]
        public void when_clicking_next_page_if_there_is_no_excess_data_it_will_not_change_page()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.NextPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Product Number 0"));
        }

        [Test]
        public void when_clicking_previous_page_if_there_is_not_a_previous_page_of_data_it_will_not_change_page()
        {
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count, Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        [Test]
        public void when_clicking_previous_page_after_next_page_it_returns_to_the_actual_previous_page_of_data()
        {
            _viewModel.NextPage.Execute(null);
            _viewModel.PreviousPage.Execute(null);
            Assert.That(_viewModel.ProductsList.Count(), Is.EqualTo(100));
            Assert.That(_viewModel.ProductsList[0].Description, Is.EqualTo("Another thing 0"));
            Assert.That(_viewModel.ProductsList[99].Description, Is.EqualTo("Another thing 99"));
        }

        [Test]
        public void when_window_is_active_then_it_refreshes_the_data()
        {
            string newProductPositionedFirst = "AAA Product";
            var productDescription = new ProductDescription
            {
                Description = newProductPositionedFirst
            };
            using (var session = _dataStorage.CreateSession())
            {
                session.Store(productDescription);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.ProductsList.Any(c => c.Description.Equals(newProductPositionedFirst)), Is.True);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_product_is_deleted_elsewhere_then_the_selected_product_fields_are_emptied()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToDelete = session.Load<ProductDescription>(selectedProductId);
                session.Delete(itemToDelete);
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.Description, Is.Null);
        }

        [Test]
        public void when_refreshing_data_if_the_selected_product_has_changed_then_the_field_is_updated()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            using (var session = _dataStorage.CreateSession())
            {
                var itemToEdit = session.Load<ProductDescription>(selectedProductId);
                itemToEdit.Description = "A New Description";
                session.SaveChanges();
            }
            _viewModel.Refresh.Execute(null);
            Assert.That(_viewModel.Description, Is.EqualTo("A New Description"));
        }

        [Test]
        public void when_editing_a_product_and_clicking_save_it_refreshes_the_data_in_the_grid_and_empties_the_editing_controls()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            _viewModel.Description = "AAA New Description";

            _viewModel.Save.Execute(null);
            Assert.That(_viewModel.ProductsList.Any(c => c.Description.Equals("AAA New Description")), Is.True);
            Assert.That(string.IsNullOrWhiteSpace(_viewModel.Description), Is.True);
        }

        [Test]
        public void when_the_product_is_not_selected_the_delete_button_is_disabled()
        {
            _viewModel.LoadSelectedProduct(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_product_with_no_associated_positions_is_selected_the_delete_button_is_enabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_product_with_at_least_one_associated_saleconfirmation_is_selected_the_delete_button_is_disabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            using (var session = _dataStorage.CreateSession())
            {
                var product = session.Load<ProductDescription>(selectedProductId);
                var sc = new SaleConfirmation();
                sc.ProductDetails.Add(new ProductDetails { ProductId = selectedProductId, Description = product.Description});
                session.Store(sc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_product_with_at_least_one_associated_loadingDocument_is_selected_the_delete_button_is_disabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            using (var session = _dataStorage.CreateSession())
            {
                var product = session.Load<ProductDescription>(selectedProductId);
                var ld = new LoadingDocument();
                ld.ProductDetails.Add(new ProductDetails { ProductId = product.Id, Description = product.Description});
                session.Store(ld);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_product_with_at_least_one_associated_priceConfirmation_is_selected_the_delete_button_is_disabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            using (var session = _dataStorage.CreateSession())
            {
                var product = session.Load<ProductDescription>(selectedProductId);
                var pc = new PriceConfirmation();
                pc.ProductDetails.Add(new ProductDetails { ProductId = product.Id, Description = product.Description });
                session.Store(pc);
                session.SaveChanges();
            }
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_no_product_is_selected_the_save_button_is_disabled()
        {
            _viewModel.LoadSelectedProduct(null);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_product_is_selected_but_not_edited_the_save_button_is_disabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_product_is_edited_the_save_button_is_enabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            _viewModel.Description = "Some New Description";
            Assert.That(_viewModel.SaveButtonEnabled, Is.True);
        }

        [Test]
        public void when_a_new_product_is_created_the_save_button_and_delete_button_are_disabled()
        {
            _viewModel.CreateNew.Execute(null);
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_a_product_does_not_have_a_Description_then_the_save_button_is_disabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            _viewModel.Description = string.Empty;
            Assert.That(_viewModel.SaveButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_selected_product_is_new_the_delete_button_is_disabled_even_when_it_has_a_description()
        {
            _viewModel.CreateNew.Execute(null);
            _viewModel.Description = "Somethingsomething";
            Assert.That(_viewModel.DeleteButtonEnabled, Is.False);
        }

        [Test]
        public void when_the_delete_button_is_pressed_it_deletes_the_product_and_refresh_the_grid()
        {
            var stringId = _viewModel.ProductsList[0].Id;
            var selectedProductId = int.Parse(stringId.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            _viewModel.DeleteProduct.Execute(null);
            Assert.That(_viewModel.ProductsList.Any(x => x.Id.Equals(stringId)), Is.False);
        }

        [Test]
        public void when_selected_product_is_null_edit_controls_are_disabled()
        {
            _viewModel.LoadSelectedProduct(null);
            Assert.That(_viewModel.EditControlsEnabled, Is.False);
        }

        [Test]
        public void when_selected_product_is_new_customer_edit_controls_are_enabled()
        {
            _viewModel.CreateNew.Execute(null);
            Assert.That(_viewModel.EditControlsEnabled, Is.True);
        }

        [Test]
        public void when_selected_product_is_existing_product_edit_controls_are_enabled()
        {
            var selectedProductId = int.Parse(_viewModel.ProductsList[0].Id.Split('/')[1]);
            _viewModel.LoadSelectedProduct(selectedProductId);
            Assert.That(_viewModel.EditControlsEnabled, Is.True);
        }

        private void InsertInitialData()
        {
            using (var session = _dataStorage.CreateSession())
            {
                for (int i = 99; i >= 0; i--)
                {
                    var newProduct = new ProductDescription
                    {
                        Description = "Product Number " + i,
                    };
                    session.Store(newProduct);
                }
                for (int i = 0; i < 100; i++)
                {
                    var newProduct = new ProductDescription
                    {
                        Description = "Another thing " + i,
                    };
                    session.Store(newProduct);
                }
                session.SaveChanges();
            }
        }

        private RavenDataStorage _dataStorage;
        private Mock<IWindowManager> _mockWindowManager;
        private ProductsWindowGridViewModel _viewModel;
    }
}
