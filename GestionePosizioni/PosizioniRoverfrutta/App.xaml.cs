using System.Windows;
using PosizioniRoverfrutta.Windows;
using QueryManager;

namespace PosizioniRoverfrutta
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private WindowManager _windowManager;
        public IDataStorage DataStorage { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeWindowManager();

            InitializeDataStorage();

            SetApplicationBehaviour();

            ShowMainWindow();
        }

        private void InitializeWindowManager()
        {
            _windowManager = new WindowManager(DataStorage);
            _windowManager.RegisterWindowClass(WindowTypes.ConfermaVendita, typeof(DocumentWindow));
        }

        private void ShowMainWindow()
        {
            MainWindow = new MainWindow(_windowManager);
            MainWindow.Show();
        }

        private void SetApplicationBehaviour()
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private void InitializeDataStorage()
        {
            DataStorage = new RavenDataStorage();
            DataStorage.Initialize();
        }
    }
}
