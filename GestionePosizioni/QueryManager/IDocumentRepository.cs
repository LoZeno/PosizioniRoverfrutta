using System;

namespace QueryManager
{
    public interface IDocumentRepository<T> : IDisposable
    {
        string Add(T customer);
        T FindById(string custId);
        void Save();
    }
}