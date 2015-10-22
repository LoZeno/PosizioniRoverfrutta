using System;
using System.Collections.Generic;
using System.Linq;
using CustomWPFControls;
using Models.Entities;
using QueryManager;
using Raven.Client.Linq;

namespace PosizioniRoverfrutta.Services
{
    internal class TermsOfPaymentAutoCompleteBoxProvider : IAutoCompleteBoxDataProvider
    {
        public TermsOfPaymentAutoCompleteBoxProvider(IDataStorage dataStorage)
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
                    results = session.Query<TermOfPayment>()
                        .Where(tp => tp.Description.StartsWith(textPattern, StringComparison.CurrentCultureIgnoreCase))
                        .OrderBy(p => p.Description)
                        .Take(10)
                        .Select(p => p.Description)
                        .ToList();
                }
            }
            return results ?? new List<string>();
        }

        private readonly IDataStorage _dataStorage;
    }
}