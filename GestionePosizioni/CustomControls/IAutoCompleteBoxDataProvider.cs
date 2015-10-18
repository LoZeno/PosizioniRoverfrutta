using System.Collections.Generic;

namespace CustomWPFControls
{
    public interface IAutoCompleteBoxDataProvider
    {
        IEnumerable<string> GetItems(string textPattern);
    }
}