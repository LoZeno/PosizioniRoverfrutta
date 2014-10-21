using System.Windows;
using Models;
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

            SetApplicationBehaviour();

            InitializeDataStorage();

            InitializeWindowManager();

            ShowMainWindow();
        }

        private void InitializeWindowManager()
        {
            _windowManager = new WindowManager(DataStorage);
            _windowManager.RegisterWindowClass(WindowTypes.ConfermaVendita, typeof(DocumentWindow));
            _windowManager.RegisterWindowClass(WindowTypes.ElencoPosizioni, typeof(ListPositions));
            _windowManager.RegisterWindowClass(WindowTypes.DistintaCarico, typeof(LoadingDocumentWindow));
            _windowManager.RegisterWindowClass(WindowTypes.InviaEmail, typeof(SendGmailAttachment));
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

            InitializeDefaultData();
        }

        private void InitializeDefaultData()
        {
            using (var session = DataStorage.CreateSession())
            {
                var defaults = session.Load<DefaultValues>(1);
                if (defaults == null)
                {
                    defaults = new DefaultValues {Id = 1, Vat = 4};
                    session.Store(defaults);
                    session.SaveChanges();
                }
            }
        }
    }
}
