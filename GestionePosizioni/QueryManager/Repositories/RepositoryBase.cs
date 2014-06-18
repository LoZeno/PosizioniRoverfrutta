using Raven.Client;

namespace QueryManager.Repositories
{
    public abstract class RepositoryBase<T>
    {
        protected IDocumentSession Session;

        public RepositoryBase(IDocumentSession session)
        {
            Session = session;
        }

        public abstract string Add(T entity);

        public T FindById(string custId)
        {
            return Session.Load<T>(custId);
        }

        public void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public void Save()
        {
            Session.SaveChanges();
        }

        public IDocumentSession DatabaseSession
        {
            get
            {
                return Session;
            }
            set
            {
                Session = value;
            }
        }

    }
}