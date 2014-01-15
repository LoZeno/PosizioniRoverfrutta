using System;
using System.Collections.Generic;

namespace dragonz.actb.provider
{
    public class SimpleStaticDataProvider : IAutoCompleteDataProvider
    {
        private IEnumerable<string> _source;

        public SimpleStaticDataProvider(IEnumerable<string> source)
        {
            _source = source;
        }

        public IEnumerable<string> GetItems(string textPattern)
        {
            foreach (var item in _source)
            {
                if (item.ToLower().Contains(textPattern.ToLower()))
                {
                    yield return item;
                }
            }
        }
    }
}