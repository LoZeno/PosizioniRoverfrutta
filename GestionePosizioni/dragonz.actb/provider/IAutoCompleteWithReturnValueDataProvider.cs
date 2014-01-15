using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dragonz.actb.provider
{
    public interface IAutoCompleteWithReturnValueDataProvider : IAutoCompleteDataProvider
    {
        object GetValue(string selectedText);
    }
}
