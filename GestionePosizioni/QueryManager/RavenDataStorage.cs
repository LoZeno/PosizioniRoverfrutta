using System;
using System.IO;
using Raven.Client.Embedded;

namespace QueryManager
{
    public class RavenDataStorage : IDataStorage
    {
        private string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Posizioni\Archive");

        private EmbeddableDocumentStore documentStore;

        public void Initialize()
        {
            documentStore = new EmbeddableDocumentStore { DataDirectory = ConnectionString };
            documentStore.Initialize();
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
