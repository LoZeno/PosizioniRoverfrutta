using System.Collections.Generic;
using System.Linq;
using CustomWPFControls;
using Models.Companies;
using QueryManager;
using QueryManager.QueryHelpers;

namespace PosizioniRoverfrutta.Services
{
    class CustomerNamesAutoCompleteBoxProvider<T> : IAutoCompleteBoxDataProvider where T : CompanyBase
    {
        public CustomerNamesAutoCompleteBoxProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public IEnumerable<string> GetItems(string textPattern)
        {
            IEnumerable<string> results = null;
            if (textPattern.Length > 3)
            {
                using (var session = _dataStorage.CreateSession())
                {
                    results = session.FindByPartialName<T>(textPattern).Take(30).Select(c => c.CompanyName).OrderBy(x => x);
                }
            }
            return results ?? new List<string>();
        }

        private readonly IDataStorage _dataStorage;
    }
}