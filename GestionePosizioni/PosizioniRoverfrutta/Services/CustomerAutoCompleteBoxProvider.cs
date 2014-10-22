using System.Collections.Generic;
using System.Linq;
using dragonz.actb.provider;
using Models;
using Models.Companies;
using QueryManager;
using QueryManager.QueryHelpers;

namespace PosizioniRoverfrutta.Services
{
    class CustomerAutoCompleteBoxProvider<T> : IAutoCompleteWithReturnValueDataProvider where T : CompanyBase, new()
    {
        private readonly IDataStorage _dataStorage;
        private Dictionary<string, T> _customerTemporaryStorage = new Dictionary<string, T>();


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
            if (textPattern.Length > 3)
            {
                using (var newSession = _dataStorage.CreateSession())
                {
                    _customerTemporaryStorage = newSession.FindByPartialName<T>(textPattern)
                        .Take(10)
                        .ToDictionary(x => x.CompanyName, x => x);
                }
            }
            else
            {
                _customerTemporaryStorage = new Dictionary<string, T>();
            }
            if (!_customerTemporaryStorage.ContainsKey(textPattern))
                _customerTemporaryStorage.Add(textPattern, new T{CompanyName = textPattern});
            
            return _customerTemporaryStorage.Keys;
        }
    }
}