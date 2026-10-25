using System.Collections.Generic;

namespace CustomWPFControls;

public interface IAutoCompleteWithReturnValueDataProvider
{
    IEnumerable<string> GetItems(string textPattern, out object selectedItem);
}
