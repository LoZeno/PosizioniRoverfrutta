using System.Collections.Generic;
using System.Linq;
using System.Windows;
using dragonz.actb.provider;
using Models;
using QueryManager;
using QueryManager.Repositories;
using Raven.Client;

namespace GestionePosizioni.CustomControls.ControlServices
{
    public class CustomerAutoCompleteBoxProvider : IAutoCompleteWithReturnValueDataProvider
    {
        private ICustomerRepository _repository;
        private Dictionary<string, Customer> _customerTemporaryStorage = new Dictionary<string, Customer>();

        private IDataStorage DataStorage
        {
            get { return Bootstrapper.Instance.Resolve<IDataStorage>(); }
        }

        private IDocumentSession OpenSession()
        {
            return DataStorage.DocumentStore.OpenSession();
        }

        public CustomerAutoCompleteBoxProvider(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public object GetValue(string selectedText)
        {
            if (_customerTemporaryStorage.ContainsKey(selectedText))
            {
                return _customerTemporaryStorage[selectedText];
            }
            return null;
        }

        public IEnumerable<string> GetItems(string textPattern)
        {
            if (textPattern.Length > 2)
            {
                using (var newSession = OpenSession())
                {
                    _repository.DatabaseSession = newSession;
            
                    _customerTemporaryStorage = _repository.FindByPartialName(textPattern)
                        .Take(30)
                        .ToDictionary(x => x.CompanyName, x => x);
                }
            }
            else
            {
                _customerTemporaryStorage = new Dictionary<string, Customer>();    
            }
            return _customerTemporaryStorage.Keys;
        }
    }
}
