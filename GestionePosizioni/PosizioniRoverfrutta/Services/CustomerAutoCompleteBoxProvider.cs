using System.Collections.Generic;
using System.Linq;
using dragonz.actb.provider;
using Models;
using QueryManager;
using QueryManager.QueryHelpers;

namespace PosizioniRoverfrutta.Services
{
    class CustomerAutoCompleteBoxProvider : IAutoCompleteWithReturnValueDataProvider
    {
        private readonly IDataStorage _dataStorage;
        private Dictionary<string, Customer> _customerTemporaryStorage = new Dictionary<string, Customer>();


        public CustomerAutoCompleteBoxProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
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
            if (textPattern.Length > 4)
            {
                using (var newSession = _dataStorage.CreateSession())
                {
                    _customerTemporaryStorage = newSession.FindByPartialName<Customer>(textPattern)
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