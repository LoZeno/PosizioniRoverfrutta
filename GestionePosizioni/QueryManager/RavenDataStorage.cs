using System;
using System.Configuration;
using System.IO;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace QueryManager
{
    public class RavenDataStorage : IDataStorage
    {
        private string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Posizioni\Archive");

        private EmbeddableDocumentStore documentStore;

        /// <summary>
        /// Initializes the database and creates the necessary indexes
        /// </summary>
        public void Initialize()
        {
            documentStore = new EmbeddableDocumentStore { DataDirectory = ConnectionString};
            documentStore.Initialize();
            CreateIndexes();

            string alwaysWaitForLastWrite = ConfigurationManager.AppSettings["AlwaysWaitForLastWrite"];
            if ("True" == alwaysWaitForLastWrite)
            {
                documentStore.Conventions.DefaultQueryingConsistency =
                    ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            }
        }

        private void CreateIndexes()
        {
            IndexCreation.CreateIndexes(this.GetType().Assembly, documentStore);
        }

        public string ConnectionString
        {
            get
            {
                return dataDirectory;
            }
            set
            {
                dataDirectory = value;
            }
        }

        public EmbeddableDocumentStore DocumentStore
        {
            get { return documentStore; }
        }
    }
}
