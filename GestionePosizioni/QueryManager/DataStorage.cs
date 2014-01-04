using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Embedded;

namespace QueryManager
{
    public class DataStorage : IDataStorage
    {
        private EmbeddableDocumentStore documentStore;

        public void Initialize()
        {
            documentStore = new EmbeddableDocumentStore { DataDirectory = ConnectionString };
            documentStore.Initialize();
        }

        public string ConnectionString { get; set; }

        public EmbeddableDocumentStore DocumentStore
        {
            get { return documentStore; }
        }
    }
}
