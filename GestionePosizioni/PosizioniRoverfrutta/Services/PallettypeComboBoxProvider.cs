using CustomWPFControls;
using Models.Entities;
using QueryManager;
using Raven.Client.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System;

namespace PosizioniRoverfrutta.Services
{

    internal class PalletTypesComboBoxProvider : IAutoCompleteBoxDataProvider
    {
        public PalletTypesComboBoxProvider()
        {
            _dataStorage = ((App)Application.Current).DataStorage;
        }

        public IEnumerable<string> GetItems(string textPattern)
        {
            using (var session = _dataStorage.CreateSession())
            {
                var baseQuery = session.Query<PalletType>()
                    .OrderBy(p => p.Name)
                    .Select(p => p.Name);
                if (string.IsNullOrWhiteSpace(textPattern))
                {
                    return baseQuery.ToList();
                }
                return baseQuery.ToList().Where(p => p.ToLowerInvariant().Contains(textPattern.ToLowerInvariant()));
            }
        }

        public IEnumerable<string> GetItems()
        {
            return GetItems(string.Empty);
        }

        private IDataStorage _dataStorage;
    }
}
