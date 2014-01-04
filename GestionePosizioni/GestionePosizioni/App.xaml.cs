using System.Windows;
using Microsoft.Practices.Unity;
using QueryManager;

namespace GestionePosizioni
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IDataStorage _dataStorage;

        public IDataStorage DataStorage {
            get { return _dataStorage; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var iocContainer = new UnityContainer();
            iocContainer.RegisterType<IDataStorage, RavenDataStorage>();
            
            //Initialize DataStorage
            _dataStorage = iocContainer.Resolve<IDataStorage>();
            _dataStorage.Initialize();

            //Show main window
            var mainWindow = iocContainer.Resolve<MainWindow>(); // Creating Main window
            mainWindow.Show();
        }
    }
}
