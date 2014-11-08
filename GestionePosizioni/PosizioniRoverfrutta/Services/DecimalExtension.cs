using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosizioniRoverfrutta.Services
{
    internal static class DecimalExtension
    {
        public static decimal RoundUp(this decimal input, int places)
        {
            var multiplier = (decimal)Math.Pow(10, places);
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
