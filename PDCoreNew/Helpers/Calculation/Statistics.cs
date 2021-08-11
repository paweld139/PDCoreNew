using PDCoreNew.Utils;
using System;

namespace PDCoreNew.Helpers.Calculation
{
    public class Statistics
    {
        public virtual double Average
        {
            get
            {
                return Total / Count;
            }
        }

        public double Max { get; private set; }
        public double Min { get; private set; }

        public char Letter
        {
            get
            {
                return Average switch
                {
                    var d when d >= 90.0 => 'A',
                    var d when d >= 80.0 => 'B',
                    var d when d >= 70.0 => 'C',
                    var d when d >= 60.0 => 'D',
                    _ => 'F',
                };
            }
        }

        public double Total { get; private set; }
        public int Count { get; private set; }

        public void Add(double number)
        {
            Total += number;
            Count++;
            Min = Math.Min(number, Min);
            Max = Math.Max(number, Max);
        }

        public Statistics()
        {
            Count = 0;
            Total = 0.0;
            Max = double.MinValue;
            Min = double.MaxValue;
        }

        public override string ToString() => ReflectionUtils.GetSummary(this, 2);
    }
}
