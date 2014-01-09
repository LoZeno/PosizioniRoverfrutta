using System.Net.Mime;
using GestionePosizioni.ViewModels;
using NUnit.Framework;

namespace GestionePosizioniTests
{
    [TestFixture]
    public class MainViewModelTest
    {
        private MainViewModel _viewModel;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _viewModel = new MainViewModel();
        }
    }
}
