using System;
using System.Configuration;
using System.IO;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace QueryManager
{
    public class RavenDataStorage : IDataStorage
    {
        private string _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Posizioni\Archive");

        private EmbeddableDocumentStore _documentStore;

        /// <summary>
        /// Initializes the database and creates the necessary indexes
        /// </summary>
        public void Initialize()
        {
            _documentStore = new EmbeddableDocumentStore { DataDirectory = ConnectionString};
            _documentStore.Initialize();
            CreateIndexes();

            string alwaysWaitForLastWrite = ConfigurationManager.AppSettings["AlwaysWaitForLastWrite"];
            if ("True" == alwaysWaitForLastWrite)
            {
                _documentStore.Conventions.DefaultQueryingConsistency =
                    ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            }
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

        public EmbeddableDocumentStore DocumentStore
        {
            get { return _documentStore; }
        }

        public IDocumentSession CreateSession()
        {
            return _documentStore.OpenSession();
        }
    }
}
