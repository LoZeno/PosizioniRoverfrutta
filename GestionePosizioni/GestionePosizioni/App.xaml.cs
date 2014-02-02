using System.Windows;
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

            var bootstrapper = Bootstrapper.Instance;
            _dataStorage = bootstrapper.Resolve<IDataStorage>();

            //Show main window
            var mainWindow = bootstrapper.Resolve<MainWindow>(); // Creating Main window
            mainWindow.Show();
        }
    }
}
