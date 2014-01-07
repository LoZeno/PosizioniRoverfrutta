using System;
using Models;
using Raven.Client;
using Raven.Client.Document;

namespace QueryManager
{
    public abstract class RepositoryBase<T> : IDisposable
    {
        protected IDocumentSession _session;

        public RepositoryBase(DocumentStore storage)
        {
            _session = storage.OpenSession();
        }

        public abstract string Add(T entity);

        public T FindById(string custID)
        {
            return _session.Load<T>(custID);
        }

        public void Save()
        {
            _session.SaveChanges();
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}