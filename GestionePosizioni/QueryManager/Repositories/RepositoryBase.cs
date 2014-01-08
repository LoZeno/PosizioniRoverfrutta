using Raven.Client;

namespace QueryManager.Repositories
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

        public void Delete(T entity)
        {
            _session.Delete(entity);
        }

        public void Save()
        {
            _session.SaveChanges();
        }

    }
}