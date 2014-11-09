using System;

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
