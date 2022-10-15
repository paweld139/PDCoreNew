using System;

namespace PDCoreNew.Extensions
{
    public static class DecimalExtensions
    {
        public static int GetDecimalPlacesAmount(this decimal input)
        {
            var bits = decimal.GetBits(input);

            int getBit = bits[3];

            var bytes = BitConverter.GetBytes(getBit);

            return bytes[2];
        }
    }
}
