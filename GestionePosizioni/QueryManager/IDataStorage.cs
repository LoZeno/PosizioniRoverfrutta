using Raven.Client;
using Raven.Client.Embedded;

namespace QueryManager
{
    public interface IDataStorage
    {
        void Initialize();
        string ConnectionString { get; set; }
        EmbeddableDocumentStore DocumentStore { get; }
        IDocumentSession CreateSession();
        void StartBackup(string path, bool incremental);
    }
}