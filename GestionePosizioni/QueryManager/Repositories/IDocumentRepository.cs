using System;

namespace QueryManager.Repositories
{
    public interface IDocumentRepository<T> : IDisposable
    {
        string Add(T customer);
        T FindById(string custId);
        void Save();
    }
}