using System.IO;
using System.Windows;
using Models.Entities;
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
        private readonly string _tempFolder = Path.Combine(Path.GetTempPath(), "RoverfruttaAttachment");
        public IDataStorage DataStorage { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetApplicationBehaviour();

            InitializeDataStorage();

            InitializeWindowManager();

            ShowMainWindow();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DeleteTemporaryFolder();

            if (DataStorage.DocumentStore == null || DataStorage.DocumentStore.WasDisposed)
                return;
            DataStorage.DocumentStore.Dispose();
        }

        private void DeleteTemporaryFolder()
        {
            if (!Directory.Exists(_tempFolder)) return;
            Directory.Delete(_tempFolder, true);
        }

        private void InitializeWindowManager()
        {
            _windowManager = new WindowManager(DataStorage);
            _windowManager.RegisterWindowClass(WindowTypes.ConfermaVendita, typeof(DocumentWindow));
            _windowManager.RegisterWindowClass(WindowTypes.DistintaCarico, typeof(LoadingDocumentWindow));
            _windowManager.RegisterWindowClass(WindowTypes.ConfermaPrezzi, typeof(PriceConfirmationWindow));
            _windowManager.RegisterWindowClass(WindowTypes.Riepiloghi, typeof(SummaryAndInvoicesWindow));
        }

        private void ShowMainWindow()
        {
            MainWindow = new MainWindow(_windowManager, DataStorage);
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
                    defaults = new DefaultValues
                    {
                        Id = 1, 
                        Vat = 4, 
                        InvoiceVat = 22,
                        Witholding = 23
                    };
                    session.Store(defaults);
                    session.SaveChanges();
                }
            }
        }
    }
}
