using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomWPFControls
{
    public class AutoCompleteBoxAsync : AutoCompleteBox
    {
        public AutoCompleteBoxAsync()
        {
            this.Populating += AutoCompleteBoxAsync_Populating;
        }

        private void AutoCompleteBoxAsync_Populating(object sender, PopulatingEventArgs e)
        {
            var populateInfo = new PopulateInfo
            {
                AutoCompleteBox = sender as AutoCompleteBox,
                SearchText = (sender as AutoCompleteBox).SearchText,
            };
            e.Cancel = true;
            var ui = TaskScheduler.FromCurrentSynchronizationContext();
            var populate = Task.Factory.StartNew<PopulateInfo>(() => Populate(populateInfo));
            populate.ContinueWith(task => OnPopulateComplete(task.Result), ui);
        }

        public IAutoCompleteBoxDataProvider DataProvider { get; set; }


        private PopulateInfo Populate(PopulateInfo populateInfo)
        {
            populateInfo.Results = DataProvider.GetItems(populateInfo.SearchText);
            return populateInfo;
        }

        private void OnPopulateComplete(PopulateInfo populateInfo)
        {
            if (populateInfo.SearchText == populateInfo.AutoCompleteBox.SearchText)
            {
                populateInfo.AutoCompleteBox.ItemsSource = populateInfo.Results;
                populateInfo.AutoCompleteBox.PopulateComplete();
            }
        }

        private class PopulateInfo
        {
            public AutoCompleteBox AutoCompleteBox { get; set; }
            public string SearchText { get; set; }
            public IEnumerable<string> Results { get; set; }
        }
    }
}
