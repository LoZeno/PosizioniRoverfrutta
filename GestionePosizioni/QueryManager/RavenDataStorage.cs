using System;
using System.Configuration;
using System.IO;
using Models.DocumentTypes;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Backups;
using Raven.Client.Documents.Session;
using Raven.Embedded;
using Sparrow;

namespace QueryManager;

public class RavenDataStorage : IDataStorage
{
    private static readonly object ServerSync = new();
    private static string _dataDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        ConfigurationManager.AppSettings["DatabaseFolderName"] ?? "Roverfrutta",
        "Archive");

    private IDocumentStore _documentStore;
    private bool _waitForNonStaleResults;

    public void Initialize()
    {
        var turnOnManagementStudio = ConfigurationManager.AppSettings["ActivateManagementStudio"];
        var alwaysWaitForLastWrite = ConfigurationManager.AppSettings["AlwaysWaitForLastWrite"];
        var runInMemory = string.Equals(ConfigurationManager.AppSettings["Raven/RunInMemory"], "True", StringComparison.OrdinalIgnoreCase);
        _waitForNonStaleResults = string.Equals(alwaysWaitForLastWrite, "True", StringComparison.OrdinalIgnoreCase);

        var options = new ServerOptions
        {
            DataDirectory = runInMemory
                ? Path.Combine(Path.GetTempPath(), "RoverfruttaEmbedded")
                : ConnectionString,
            FrameworkVersion = "10.0.13",
            ServerUrl = string.Equals(turnOnManagementStudio, "True", StringComparison.OrdinalIgnoreCase)
                ? "http://127.0.0.1:8080"
                : "http://127.0.0.1:0"
        };
        if (runInMemory)
            options.CommandLineArgs.Add("--RunInMemory=true");

        Directory.CreateDirectory(options.DataDirectory);

        lock (ServerSync)
        {
            try
            {
                EmbeddedServer.Instance.StartServer(options);
            }
            catch (InvalidOperationException ex) when (IsAlreadyStarted(ex))
            {
                // process-wide singleton
            }
            catch (InvalidOperationException)
            {
                options.DataDirectory = Path.Combine(Path.GetTempPath(), "RoverfruttaEmbedded", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(options.DataDirectory);
                try
                {
                    EmbeddedServer.Instance.StartServer(options);
                }
                catch (InvalidOperationException retry) when (IsAlreadyStarted(retry))
                {
                }
            }
        }

        var conventions = new DocumentConventions
        {
            MaxHttpCacheSize = new Size(0, SizeUnit.Bytes)
        };
        RegisterProgressiveIdConvention<SaleConfirmation>(conventions);
        RegisterProgressiveIdConvention<SummaryAndInvoice>(conventions);

        var databaseName = runInMemory
            ? "Archive-" + Guid.NewGuid().ToString("N")
            : "Archive";

        _documentStore = EmbeddedServer.Instance.GetDocumentStore(new DatabaseOptions(databaseName)
        {
            Conventions = conventions
        });

        if (_waitForNonStaleResults)
        {
            _documentStore.OnBeforeQuery += (_, args) =>
                args.QueryCustomization.WaitForNonStaleResults();
        }

        IndexCreation.CreateIndexes(GetType().Assembly, _documentStore);
    }

    private static bool IsAlreadyStarted(InvalidOperationException ex) =>
        ex.Message.IndexOf("already started", StringComparison.OrdinalIgnoreCase) >= 0;

    private static void RegisterProgressiveIdConvention<T>(DocumentConventions conventions)
    {
        // Trailing '|' requests a server identity so IDs become Collection/1, Collection/2, ...
        conventions.RegisterAsyncIdConvention<T>(
            (_, entity) => System.Threading.Tasks.Task.FromResult(
                conventions.GetCollectionName(entity.GetType()) + "|"));
    }

    public void StartBackup(string path, bool incremental)
    {
        _documentStore.Maintenance.Send(new BackupOperation(new BackupConfiguration
        {
            LocalSettings = new LocalSettings { FolderPath = path },
            BackupType = incremental ? BackupType.Backup : BackupType.Snapshot
        }));
    }

    public string ConnectionString
    {
        get => _dataDirectory;
        set => _dataDirectory = value;
    }

    public IDocumentStore DocumentStore => _documentStore;

    public IDocumentSession CreateSession()
    {
        return _documentStore.OpenSession();
    }
}
