﻿using System.Collections.Generic;

namespace PDCore.Models
{
    public class ChartData<TKey, TValue>
    {
        public string Title { get; set; }

        public Dictionary<string, Dictionary<TKey, TValue>> DataSeriesList { get; set; }
    }
}
