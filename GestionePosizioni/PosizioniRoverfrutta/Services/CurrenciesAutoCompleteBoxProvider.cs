using System;
using System.Collections.Generic;
using System.Linq;
using dragonz.actb.provider;
using Models;
using QueryManager;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.Services
{
    internal class CurrenciesAutoCompleteBoxProvider : IAutoCompleteDataProvider
    {
        public CurrenciesAutoCompleteBoxProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public IEnumerable<string> GetItems(string textPattern)
        {
            IEnumerable<string> results = null;
            if (textPattern.Length > 0)
            {
                using (var session = _dataStorage.CreateSession())
                {
                    results = session.Query<Currency>()
                        .Where(p => p.Name.StartsWith(textPattern, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(p => p.Name)
                        .Take(10)
                        .Select(p => p.Name)
                        .Distinct();
                }
            }
            return results ?? new List<string>();
        }

        private readonly IDataStorage _dataStorage;
    }
}