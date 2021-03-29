using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDCore.Extensions;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace PDCore.Tests
{
    [TestClass]
    public class UtilsTests
    {
        #region ObjectUtils

        [TestMethod]
        public void CanGetCallerMethodName()
        {
            string actual = Utils.ReflectionUtils.GetCallerMethodName(1);

            string expected = "CanGetCallerMethodName";


            Assert.AreEqual(expected, actual);
        }

        #endregion


        #region SqlUtils

        [TestMethod]
        public void CanGetConnectionStringByNameOrConnectionString()
        {
            string[] texts =
            {
                "Main",
                "DefaultConnection",
                "Medic4You",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MainTest;User ID=sa;Password=hasloos",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MedicSylwia;User ID=sa;Password=hasloos"
            };

            var actual = texts.Select(Utils.SqlUtils.GetConnectionString);

            var expected = new[] { null, texts[3], texts[4], texts[3], texts[4] };


            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void CanTestConnectionString()
        {
            string[] texts =
            {
                "Main",
                "DefaultConnection",
                "Medic4You",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MainTest;User ID=sa;Password=hasloos",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MedicSylwia;User ID=sa;Password=hasloos",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MainTest;User ID=sa;Password=hasloos2"
            };

            int i = 0;

            Tuple<bool, int> func(string t, string p) => Tuple.Create(Common.Utils.SqlUtils.TestConnectionString(t, p), i++);

            var actual = texts.Select(t => func(t, null).Item1).ToList();

            var actual2 = texts.Select(t => func(t, "System.Data.SqlClient").Item1);


            Assert.IsTrue(actual.SequenceEqual(actual2));


            var expected = new[] { false, false, false, true, true, false };


            Assert.IsTrue(expected.SequenceEqual(actual));

            Assert.AreEqual(12, i);
        }

        [TestMethod]
        public void CanGetDefaultSchema()
        {
            string[] connectionStrings =
            {
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MainTest;User ID=sa;Password=hasloos",
                "Data Source=LAPTOP-JHQ9SF1E\\SQLEXPRESSS;Initial Catalog=MedicSylwia;User ID=sa;Password=hasloos"
            };

            var sqlConnections = connectionStrings.Select(s => Common.Utils.SqlUtils.GetDbConnection(s, false)).Cast<SqlConnection>();

            var schemas = sqlConnections.Select(Utils.SqlUtils.GetDefaultSchema);

            var expected = new[] { "dbo", "dbo" };

            Assert.IsTrue(expected.SequenceEqual(schemas));
        }

        #endregion


        #region IOUtils

        [TestMethod]
        public void CanGetFileCountForDirectory()
        {
            string[] directories = { @"C:\Fraps", @"C:\Frapso" };

            var actual = directories.Select(d => Utils.IOUtils.GetFilesCount(d)).ToArray();

            int actual2 = Utils.IOUtils.GetFilesCount(directories[0], true);

            int expected = 0;

            Assert.IsTrue(actual2 > actual[0]);
            Assert.AreEqual(expected, actual[1]);
        }

        #endregion
    }
}
