using System;
using Models;
using Raven.Client;
using Raven.Client.Document;

namespace QueryManager
{
    public abstract class RepositoryBase<T>
    {
        protected IDocumentSession _session;

        public RepositoryBase(IDocumentSession session)
        {
            _session = session;
        }

        public abstract string Add(T entity);

        public T FindById(string custID)
        {
            return _session.Load<T>(custID);
        }

    }
}