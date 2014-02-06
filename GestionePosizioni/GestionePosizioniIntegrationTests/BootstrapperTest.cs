using GestionePosizioni;
using GestionePosizioni.ViewModels;
using NUnit.Framework;
using QueryManager;
using QueryManager.Repositories;

namespace IntegrationTests
{
    [TestFixture]
    public class BootstrapperTest
    {
        [Test]
        public void should_initialize_RavenDataStorage()
        {
            var bootstrapper = Bootstrapper.Instance;
            Assert.DoesNotThrow(() => bootstrapper.Resolve<IDataStorage>());
            Assert.IsTrue(bootstrapper.Resolve<IDataStorage>().GetType() == typeof(RavenDataStorage));
        }

        [Test]
        public void should_always_give_same_instance_of_IDataStorage()
        {
            var bootstrapper = Bootstrapper.Instance;
            var firstDataStorage = bootstrapper.Resolve<IDataStorage>();
            var secondDataStorage = bootstrapper.Resolve<IDataStorage>();

            Assert.AreSame(firstDataStorage, secondDataStorage);
        }

        [Test]
        public void should_initialize_SaleConfirmationRepository()
        {
            var bootstrapper = Bootstrapper.Instance;
            Assert.IsTrue(bootstrapper.Resolve<ISaleConfirmationRepository>().GetType() == typeof(SaleConfirmationRepository));
        }

        [Test]
        public void should_initialize_CustomerRepository()
        {
            var bootstrapper = Bootstrapper.Instance;
            Assert.IsTrue(bootstrapper.Resolve<ICustomerRepository>().GetType() == typeof (CustomerRepository));
        }

        [Test]
        public void should_initialize_MainViewModel()
        {
            var bootstrapper = Bootstrapper.Instance;
            Assert.IsTrue(bootstrapper.Resolve<IMainViewModel>().GetType() == typeof(MainViewModel));
        }
    }
}
