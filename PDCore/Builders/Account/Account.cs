﻿using System;

namespace PDCore.Builders.Account
{
    public class Account
    {
        public DateTime DueDate { get; set; }
        public Customer Customer { get; set; }
        public double Balance { get; set; }
    }
}
