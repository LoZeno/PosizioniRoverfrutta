using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace QueryManager;

public interface IDataStorage
{
    void Initialize();
    string ConnectionString { get; set; }
    IDocumentStore DocumentStore { get; }
    IDocumentSession CreateSession();
    void StartBackup(string path, bool incremental);
}
