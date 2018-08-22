using System;
using System.Configuration;
using System.IO;
using Models.DocumentTypes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace QueryManager
{
    public class RavenDataStorage : IDataStorage
    {
        private static string _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ConfigurationManager.AppSettings["DatabaseFolderName"], @"Archive");

        private EmbeddableDocumentStore _documentStore;

        /// <summary>
        /// Initializes the database and creates the necessary indexes
        /// </summary>
        public void Initialize()
        {
            _documentStore = new EmbeddableDocumentStore
            {
                DataDirectory = ConnectionString,
            };

            var turnOnManagementStudio = ConfigurationManager.AppSettings["ActivateManagementStudio"];
            if ("True" == turnOnManagementStudio)
            {
                _documentStore.UseEmbeddedHttpServer = true;
            }

            var alwaysWaitForLastWrite = ConfigurationManager.AppSettings["AlwaysWaitForLastWrite"];
            if ("True" == alwaysWaitForLastWrite)
            {
                _documentStore.Conventions.DefaultQueryingConsistency =
                    ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            }

            SetSaleConfirmationsToUseProgressiveIndexes();

            SetSummaryAndInvoiceTypeToUseProgressiveIndexes();

            DisableLayer2Caching();

            _documentStore.Initialize();
            _documentStore.DisableAggressiveCaching();

            CreateIndexes();
        }

        private void DisableLayer2Caching()
        {
            _documentStore.Conventions.ShouldCacheRequest = url => false;
            _documentStore.Conventions.DisableProfiling = true;
        }

        private void SetSummaryAndInvoiceTypeToUseProgressiveIndexes()
        {
            _documentStore.Conventions.RegisterIdConvention<SummaryAndInvoice>(
                (dbname, commands, entity) => _documentStore.Conventions.GetTypeTagName(entity.GetType()) + "/");
        }

        private void SetSaleConfirmationsToUseProgressiveIndexes()
        {
            _documentStore.Conventions.RegisterIdConvention<SaleConfirmation>(
                (dbname, commands, entity) => _documentStore.Conventions.GetTypeTagName(entity.GetType()) + "/");
        }

        public void StartBackup(string path, bool incremental)
        {
            _documentStore.DocumentDatabase.Maintenance.StartBackup(path, incremental, new DatabaseDocument(), new ResourceBackupState());
        }

        private void CreateIndexes()
        {
            IndexCreation.CreateIndexes(GetType().Assembly, _documentStore);
        }

        public string ConnectionString
        {
            get
            {
                return _dataDirectory;
            }
            set
            {
                _dataDirectory = value;
            }
        }

        public EmbeddableDocumentStore DocumentStore => _documentStore;

        public IDocumentSession CreateSession()
        {
            return _documentStore.OpenSession();
        }
    }
}
