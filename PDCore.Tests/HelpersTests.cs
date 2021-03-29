﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDCore.Web.Helpers.MultiLanguage;
using System;
using System.Linq;
using System.Threading;

namespace PDCore.Tests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void CanSetThreadCultureByLanguage()
        {
            string[] languages =
            {
                "pl-PL",
                "pl",
                "en",
                "en-US",
                "ja",
                "ja-JP"
            };

            var expected = new[]
            {
                Tuple.Create(languages[0], languages[1]),
                Tuple.Create(languages[0], languages[1]),
                Tuple.Create(languages[3], languages[2]),
                Tuple.Create(languages[3], languages[2]),
                Tuple.Create(languages[5], languages[2]),
                Tuple.Create(languages[5], languages[2]),
            };

            var actual = languages.Select(l =>
            {
                LanguageHelper.SetLanguage(l);

                Thread currentThread = Thread.CurrentThread;

                return Tuple.Create(currentThread.CurrentCulture.Name, currentThread.CurrentUICulture.Name);
            });

            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}
