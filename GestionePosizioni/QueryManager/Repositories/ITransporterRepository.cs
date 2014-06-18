using System.Collections.Generic;
using Models;
using Raven.Client;

namespace QueryManager.Repositories
{
    public interface ITransporterRepository
    {
        string Add(Transporter entity);
        IEnumerable<Transporter> FindByPartialName(string partialName);
        Transporter FindById(string custId);
        void Delete(Transporter entity);
        void Save();
        IDocumentSession DatabaseSession { get; set; }
    }
}