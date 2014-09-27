using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class CustomerControlViewModelTests
    {

        [SetUp]
        public void SetUp()
        {
            _viewModel = new CustomerControlViewModel();
        }

        [Test]
        public void when_initialized_returns_a_new_customer_objects()
        {
            Assert.That(_viewModel.Customer, Is.Not.Null);
        }

        private CustomerControlViewModel _viewModel;
    }
}
