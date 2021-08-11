﻿using System;

namespace PDCoreNew.Helpers.Calculation
{
    public class ObjectStatistics<T> : Statistics
    {
        public ObjectStatistics<T> Accumulate(T item, Func<T, double> propertySelector)
        {
            var itemProperty = propertySelector(item);

            Add(itemProperty);

            return this;
        }

        public ObjectStatistics<T> Compute()
        {
            _average = base.Average;

            return this;
        }

        private double _average;
        public override double Average => _average;
    }
}
