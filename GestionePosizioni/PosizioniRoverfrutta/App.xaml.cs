using System.Windows;
using QueryManager;

namespace PosizioniRoverfrutta
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IDataStorage DataStorage { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeDataStorage();

            SetApplicationBehaviour();

            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            MainWindow = new MainWindow();
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
