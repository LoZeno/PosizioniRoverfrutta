using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Models.DocumentTypes;
using Models.Entities;
using PosizioniRoverfrutta.Annotations;
using PosizioniRoverfrutta.Windows;
using QueryManager;
using QueryManager.Indexes;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace PosizioniRoverfrutta.ViewModels
{
    public class ProductsWindowGridViewModel : INotifyPropertyChanged
    {
        public ProductsWindowGridViewModel(IDataStorage dataStorage, IWindowManager windowManager)
        {
            _dataStorage = dataStorage;
            _windowManager = windowManager;
            ProductsList = new ObservableCollection<ProductRow>();

            LoadAllData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchBox
        {
            get { return _searchBox; }
            set
            {
                _searchBox = value;
                LoadAllData();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductRow> ProductsList { get; set; }

        public ICommand NextPage
        {
            get { return nextPageCommand ?? (nextPageCommand = new DelegateCommand(IncreaseSkip)); }
        }

        public ICommand PreviousPage
        {
            get { return previousPageCommand ?? (previousPageCommand = new DelegateCommand(DecreaseSkip)); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadAllData()
        {
            ProductsList.Clear();
            using (var session = _dataStorage.CreateSession())
            {
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    ProductsList.AddRange(session.Query<ProductRow, ProductsWithNumberOfDocuments>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).OrderBy(c => c.Description).Skip(skipPositions).Take(100));
                }
                else
                {
                    var customersQuery = session.Query<ProductRow, ProductsWithNumberOfDocuments>().Customize(x => x.WaitForNonStaleResultsAsOfNow());
                    var queryByName = SearchBox.Split(' ').Aggregate(customersQuery, (current, term) => current.Search(c => c.Description, "*" + term + "*", options: SearchOptions.And, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards));
                    ProductsList.AddRange(queryByName.OrderBy(c => c.Description).Take(100));
                }
            }
            if (ProductsList.Count == 0)
            {
                DecreaseSkip();
            }
        }

        private void IncreaseSkip()
        {
            if (ProductsList.Count == 100)
            {
                skipPositions += 100;
                LoadAllData();
            }
        }

        private void DecreaseSkip()
        {
            if (skipPositions != 0)
            {
                skipPositions -= 100;
                if (skipPositions < 0)
                {
                    skipPositions = 0;
                }
                LoadAllData();
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly IWindowManager _windowManager;
        private string _searchBox;
        private int skipPositions = 0;
        private ICommand nextPageCommand;
        private ICommand previousPageCommand;
    }
}
