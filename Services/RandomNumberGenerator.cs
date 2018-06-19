using System;
using System.Security.Cryptography;

namespace Hifumi.Services
{
    public static class RandomNumberGenerator
    {
        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public static int Range(int minValue, int maxValue)
        {
            byte[] randomNumber = new byte[1];
            _generator.GetBytes(randomNumber);

            double asciiVal = Convert.ToDouble(randomNumber[0]);
            double multiplier = Math.Max(0, (asciiVal / 255d) - 0.00000000001d);
            int range = maxValue - minValue + 1;

            double rangedValue = Math.Floor(multiplier * range);
            return (int)(minValue + rangedValue);
        }
    }
}
