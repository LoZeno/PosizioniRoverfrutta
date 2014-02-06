using GestionePosizioni.ViewModels;
using Microsoft.Practices.Unity;
using Models;
using QueryManager;
using QueryManager.Repositories;

namespace GestionePosizioni
{
    public class Bootstrapper
    {
        private static Bootstrapper _instance;
        private UnityContainer _iocContainer;
        private Bootstrapper()
        {
            _iocContainer = new UnityContainer();
            _iocContainer.RegisterType<IDataStorage, RavenDataStorage>();
            var dataStorage = _iocContainer.Resolve<IDataStorage>();
            dataStorage.Initialize();
            _iocContainer.RegisterInstance(dataStorage);
            _iocContainer.RegisterType<ISaleConfirmationRepository, SaleConfirmationRepository>(new InjectionConstructor(
                dataStorage.DocumentStore.OpenSession()));
            _iocContainer.RegisterType<ICustomerRepository, CustomerRepository>(
                new InjectionConstructor(dataStorage.DocumentStore.OpenSession()));
            _iocContainer.RegisterType<IMainViewModel, MainViewModel>(
                new InjectionConstructor(new SaleConfirmation(),
                    _iocContainer.Resolve<ISaleConfirmationRepository>(), _iocContainer.Resolve<ICustomerRepository>()));
            _iocContainer.RegisterType<MainWindow>(new InjectionConstructor(_iocContainer.Resolve<IMainViewModel>()));
        }

        public static Bootstrapper Instance
        {
            get { return _instance ?? (_instance = new Bootstrapper()); }
        }

        public T Resolve<T>()
        {
            return _iocContainer.Resolve<T>();
        }
    }
}
