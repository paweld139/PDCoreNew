﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDCore.Enums;
using PDCore.Extensions;
using PDCore.Utils;

namespace PDCore.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        #region StringExtension

        [TestMethod]
        public void CanAddSpacesToString()
        {
            string text = "PawełDywanYoJa";

            text = text.AddSpaces();

            string expected = "Paweł Dywan Yo Ja";

            Assert.AreEqual(expected, text);
        }

        [TestMethod]
        public void CanConvertStringToEnum()
        {
            string enumString = "WSS";

            var actual = enumString.ParseEnum<CertificateType>();

            var expected = CertificateType.WSS;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanCheckStringIsPalindrome()
        {
            var inputs = new[] { "baobab", "rotator", "tofu", "oko" };


            var actual = inputs.ConvertArray(s => s.IsPalindrome());

            var expected = new[] { false, true, false, true };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        #endregion


        #region ObjectExtension

        [TestMethod]
        public void CanCalculateSampledAverage()
        {
            var numbers = new double[] { 1, 2, 3, 4, 5, 6 };

            var actual = numbers.SampledAverage();

            var expected = 3.0;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanMultiply()
        {
            var multiplicand = 2.0;

            var multiplier = 3;

            var actual = multiplicand.Multiply(multiplier);

            var expected = 6.0;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanExtractDescriptionFromEnumValue()
        {
            CertificateType enumValue = CertificateType.WSS;

            string actual = enumValue.GetDescription();

            string expected = "Certyfikat WSS służący do podpisywania wiadomości";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanExtractEnumValueFromDescription()
        {
            string description = "Certyfikat służący do zabezpieczenia komunikacji";

            CertificateType actual = description.ToEnumValue<CertificateType>();

            CertificateType expected = CertificateType.TLS;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanCheckWhetherNumberIsDefinedInEnumType()
        {
            var numbers = new object[] { 1, 5, "2", "20" };

            var actual = numbers.Select(n => n.IsEnum<CertificateType>());

            var expected = new[] { true, false, true, false };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void CanGetCharactersFromStringSequence()
        {
            IEnumerable<string> stringSequence = new[] { "gerge", "jtweqf", "r5gw" };

            IEnumerable<char> actual = StringUtils.GetCharacters(stringSequence);

            IEnumerable<char> expected = new[] { 'g', 'e', 'r', 'g', 'e', 'j', 't', 'w', 'e', 'q', 'f', 'r', '5', 'g', 'w' };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void CanGetOrderedCharactersFromStringSequence()
        {
            IEnumerable<string> stringSequence = new[] { "gerge", "jtweqf", "r5gw" };

            IEnumerable<char> actual = StringUtils.GetOrderedCharacters(stringSequence);

            IEnumerable<char> expected = new[] { '5', 'e', 'e', 'e', 'f', 'g', 'g', 'g', 'j', 'q', 'r', 'r', 't', 'w', 'w' };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void CanGetStringSequenceWithOrderedCharactersFromStringSequence()
        {
            IEnumerable<string> stringSequence = new string[] { "abc", "bacd", "pacds" };

            IEnumerable<string> actual = StringUtils.GetWithOrderedCharacters(stringSequence);

            IEnumerable<string> expected = new[] { "abc", "abcd", "acdps" };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        #endregion


        #region DateTimeExtension

        [TestMethod]
        public void CanConvertAnyTypeOfDateFormatsToDate()
        {
            DateTime date = DateTime.Today;


            string ymd = date.ToYMD(false);

            string dmy = date.ToDMY(false);


            DateTime dateFromYmd = DateTime.Parse(ymd);

            DateTime dateFromDmy = DateTime.Parse(dmy);


            string ymdFromDmy = dmy.ToYMD(false);

            string dmyFromYmd = ymd.ToDMY(false);



            Assert.AreEqual(date, dateFromYmd);
            Assert.AreEqual(date, dateFromDmy);

            Assert.AreEqual(ymd, ymdFromDmy);
            Assert.AreEqual(dmy, dmyFromYmd);
        }

        #endregion
    }
}
