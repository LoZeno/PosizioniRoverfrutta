using NUnit.Framework;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Tests.ViewModels
{
    [TestFixture]
    public class ListPositionsViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            _dataStorage = new RavenDataStorage();
            _dataStorage.Initialize();

            CreateBasicData();

            _mainViewModel = new ListPositionsViewModel(_dataStorage);
        }

        [Test]
        public void when_no_filter_is_selected_viewmodels_returns_the_last_100_positions_ordered_by_id_descending()
        {
            
        }

        private void CreateBasicData()
        {

        }

        private RavenDataStorage _dataStorage;
        private ListPositionsViewModel _mainViewModel;
    }
}
