using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace dragonz.actb.provider
{
    public class AnotherDataProvider : IAutoCompleteDataProvider
    {
        private List<string> data = new List<string>
        {
            "One",
            "two",
            "thrEe",
            "Four",
            "FIVe",
            "Six"
        }; 

        public IEnumerable<string> GetItems(string textPattern)
        {
            return data.Where(d => d.Contains(textPattern));
        }
    }
}
