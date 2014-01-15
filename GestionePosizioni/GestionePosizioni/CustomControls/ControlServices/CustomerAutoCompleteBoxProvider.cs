using System.Collections.Generic;
using System.Linq;
using dragonz.actb.provider;
using Models;
using QueryManager.Repositories;

namespace GestionePosizioni.CustomControls.ControlServices
{
    public class CustomerAutoCompleteBoxProvider : IAutoCompleteWithReturnValueDataProvider
    {
        private ICustomerRepository _repository;
        private Dictionary<string, Customer> _customerTemporaryStorage = new Dictionary<string, Customer>(); 
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
            _customerTemporaryStorage = _repository.FindByPartialName(textPattern)
                .Take(10)
                .ToDictionary(x => x.CompanyName, x => x);
            return _customerTemporaryStorage.Keys;
        }
    }
}
