﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PDCore.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RepeatAttribute : Attribute, ITestDataSource
    {
        private readonly int _count;

        public RepeatAttribute(int count)
        {
            if (count < 2)
                throw new ArgumentOutOfRangeException(nameof(count), "Repeat count must be greater than 1.");

            _count = count - 1;
        }

        public IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            _ = testMethod;

            return Enumerable.Repeat(new object[0], _count);
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return methodInfo.Name;
        }
    }
}
