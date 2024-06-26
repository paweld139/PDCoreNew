﻿using PDCore.Utils;
using System;

namespace PDCore.Helpers.Calculation
{
    /// <summary>
    /// Liczba
    /// </summary>
    public struct Digit
    {
        private readonly byte digit;

        public Digit(byte digit)
        {
            if (digit > 9)
            {
                throw new ArgumentOutOfRangeException(ReflectionUtils.GetNameOf(() => digit), "Digit cannot be greater than nine.");
            }

            this.digit = digit;
        }

        public static implicit operator byte(Digit d)
        {
            return d.digit;
        }

        public static explicit operator Digit(byte b)
        {
            return new Digit(b);
        }

        public override string ToString()
        {
            return digit.ToString();
        }
    }
}
