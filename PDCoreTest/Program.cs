﻿using PDCore.Commands;
using PDCore.Commands.Shop;
using PDCore.Enums;
using PDCore.Extensions;
using PDCore.Factories.Fac;
using PDCore.Handlers.Payment;
using PDCore.Handlers.Payment.Receivers;
using PDCore.Helpers;
using PDCore.Helpers.DataStructures;
using PDCore.Helpers.PythonExecute;
using PDCore.Interfaces;
using PDCore.Loggers;
using PDCore.Loggers.Factory;
using PDCore.Models;
using PDCore.Models.Shop;
using PDCore.Models.Shop.Enums;
using PDCore.Processors;
using PDCore.Repositories.Repo.Shop;
using PDCore.Services.IServ;
using PDCore.Services.Serv;
using PDCore.Utils;
using PDCore.WinForms.Utils;
using PDCoreTest.Factory;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            _ = args;

            //TestCommand();

            //WriteSeparator();

            //UserProcessor();

            //WriteSeparator();

            //PaymentProcessor();

            //WriteSeparator();

            //TestEventLog();

            //WriteSeparator();

            //TestDateWordly();

            //WriteSeparator();

            //TestConfigEncryption();

            //WriteSeparator();

            //TestIronRuby();

            //WriteSeparator();

            //await TestMail();

            //WriteSeparator();

            //TestExcel();

            //WriteSeparator();

            //TestDateTime();

            //WriteSeparator();

            //TestGetSummary();

            //WriteSeparator();

            //TestAccumulator();

            //WriteSeparator();

            //TestOpenTextFile();

            //WriteSeparator();

            //TestOrderBy();

            //WriteSeparator();

            //TestCsvParsing();

            //WriteSeparator();

            //TestGetEnumValues();

            //WriteSeparator();

            //TestLoggerFactory();

            //WriteSeparator();

            //TestFactory();

            //WriteSeparator();

            //TestLogMessageFactory();

            //WriteSeparator();

            //TestNameOf();

            //WriteSeparator();

            //TestCategoryCollection();

            //WriteSeparator();

            //TestConvertCSVToDataTableAndWriteDataTable();

            //WriteSeparator();

            //TestParseCSVToObjectAndDisplayObject();

            //WriteSeparator();

            //TestWriteResultAndWriteByte();

            //WriteSeparator();

            //TestStopWatchTime();

            //WriteSeparator();

            //TestDisposableStopwatch();

            //WriteSeparator();

            //TestCacheService();

            //WriteSeparator();

            //TestObjectConvertTo();

            //WriteSeparator();

            //TestIEnumerableConvertTo();

            //WriteSeparator();

            //TestSampledAverage();

            //WriteSeparator();

            //TestMultiply();

            //WriteSeparator();

            TestIronPython();

            WriteSeparator();

            TestPython();

            WriteSeparator();

            TestPythonNet();


            Console.ReadKey();
        }

        private static void TestCommand()
        {
            var shoppingCartRepository = new ShoppingCartRepository();
            var productsRepository = new ProductsRepository();

            var product = productsRepository.FindBy("SM7B");

            var addToCartCommand = new AddToCartCommand(shoppingCartRepository,
                productsRepository,
                product);

            var increaseQuantityCommand = new ChangeQuantityCommand(
                ChangeQuantityCommand.Operation.Increase,
                shoppingCartRepository,
                productsRepository,
                product);

            var manager = new CommandManager();
            manager.Invoke(addToCartCommand);
            manager.Invoke(increaseQuantityCommand);
            manager.Invoke(increaseQuantityCommand);
            manager.Invoke(increaseQuantityCommand);
            manager.Invoke(increaseQuantityCommand);

            ShoppingCartRepository.PrintCart(shoppingCartRepository);

            manager.Undo();

            ShoppingCartRepository.PrintCart(shoppingCartRepository);
        }

        private static void UserProcessor()
        {
            var user = new User("Filip Ekberg",
                            "870101XXXX",
                            new RegionInfo("SE"),
                            new DateTimeOffset(1987, 01, 29, 00, 00, 00, TimeSpan.FromHours(2)));

            var processor = new UserProcessor();

            var result = processor.Register(user);

            Console.WriteLine(result);
        }

        private static void PaymentProcessor()
        {
            var order = new Order();
            order.LineItems.Add(new Item("ATOMOSV", "Atomos Ninja V", 499), 2);
            order.LineItems.Add(new Item("EOSR", "Canon EOS R", 1799), 1);

            order.SelectedPayments.Add(new Payment
            {
                PaymentProvider = PaymentProvider.Paypal,
                Amount = 1000
            });

            order.SelectedPayments.Add(new Payment
            {
                PaymentProvider = PaymentProvider.Invoice,
                Amount = 1797
            });

            Console.WriteLine(order.AmountDue);
            Console.WriteLine(order.ShippingStatus);

            var handler = new PaymentHandler2(
                new CreditCardHandler(),
                new InvoiceHandler(),
                new PaypalHandler()
            );

            //var handler = new PaypalHandler();
            //handler.SetNext(new CreditCardHandler()).SetNext(new InvoiceHandler());

            handler.Handle(order);

            Console.WriteLine(order.AmountDue);
            Console.WriteLine(order.ShippingStatus);
        }

        private static void TestEventLog()
        {
            EventLogLogger eventLog = new EventLogLogger(new LogMessageFactory());

            eventLog.Warn("test ojej");
            eventLog.Fatal("test ojej 2");
            eventLog.Info("test ojej 3");

            eventLog.Dispose();
        }

        private static void TestDateWordly()
        {
            DateTime date = DateTime.Now;


            string dateWordly = date.GetWordlyPL();

            ConsoleUtils.WriteResult("Data słownie", dateWordly);


            dateWordly = date.GetWordlyGB();

            ConsoleUtils.WriteResult("Data słownie", dateWordly);


            dateWordly = date.GetWordlyDE();

            ConsoleUtils.WriteResult("Data słownie", dateWordly);
        }

        private static void TestConfigEncryption()
        {
            PDCore.Common.Utils.IOUtils.ToggleConfigEncryption();
            PDCore.Common.Utils.IOUtils.ToggleConfigEncryption("system.net/mailSettings/smtp");
        }

        private static void TestIronRuby()
        {
            var variables = new Dictionary<string, object>
            {
                ["employee"] = new Employee { FirstName = "Pawl C#" }
            };

            dynamic ruby = PDCore.Common.Utils.ReflectionUtils.GetIronRubyRunitimeGlobals(variables, "Program.rb");

            dynamic person = ruby.Person.@new();

            person.firstName = "Pawl Ruby";

            person.speak();
        }

        private static Task TestMail()
        {
            SmtpSettingsModel smtpSettingsModel = new SmtpSettingsModel("p.dywan97@gmail.com", "p.dywan97@gmail.com", "Pawl", "password", "smtp.gmail.com", 587, true);

            MailMessageModel mailMessageModel = new MailMessageModel("p.dywan97@gmail.com,pawell139139@gmail.com", "Test", "Testowy", true, new[] { @"D:\Downloads\teekst.txt", @"D:\Downloads\teekst2.txt" });


            IMailServiceAsyncTask mailService = new MailServiceAsyncTask(new TraceLogger(new LogMessageFactory()));


            return mailService.SendEmailAsyncTask(mailMessageModel, smtpSettingsModel);
        }

        private static void TestExcel()
        {
            PDCore.Utils.ReflectionUtils.OpenExcelWithProcessesAndThreads();
        }

        private static void TestDateTime()
        {
            DateTime date = DateTime.Today;


            int days = date.DaysToEndOfMonth();

            ConsoleUtils.WriteResult("Dni do końca miesiąca", days);


            string dateWordly = date.GetWordlyPL();

            ConsoleUtils.WriteResult("Data słownie", dateWordly);


            long dateLong = date.GetLong(false);

            ConsoleUtils.WriteResult("Data jako liczba", dateLong);
        }

        private static void TestGetSummary()
        {
            int[] valuesInt = { 1, 6, 9, 4, 645, 4, 75 };

            var resultInt = valuesInt.Aggregate();

            Stopwatch stopWatch = new Stopwatch();

            int iterations = 10000;

            long time2 = stopWatch.TimeMillis(() => PDCore.Utils.ReflectionUtils.GetSummary2(resultInt, 2), iterations);
            long time = stopWatch.TimeMillis(() => PDCore.Utils.ReflectionUtils.GetSummary(resultInt, 2), iterations);
            long time3 = stopWatch.TimeMillis(() => PDCore.Utils.ReflectionUtils.GetSummary(new WebClient()), iterations);

            var webClientSummary = PDCore.Utils.ReflectionUtils.GetSummary(new WebClient());

            ConsoleUtils.WriteResult("GetSummary", time);
            ConsoleUtils.WriteResult("GetSummary2", time2);
            ConsoleUtils.WriteResult("GetSummary3", time3);
            ConsoleUtils.WriteResult("WebClient", webClientSummary, true);
        }

        private static void TestAccumulator()
        {
            int[] valuesInt = { 1, 6, 9, 4, 645, 4, 75 };

            double[] valuesDouble = ObjectUtils.Random().Take(10).ToArray();


            var resultInt = valuesInt.Aggregate();

            var resultDouble = valuesDouble.Aggregate();


            ConsoleUtils.WriteResult("Akumulator int", $"\n\n{resultInt}");
            Console.WriteLine();
            ConsoleUtils.WriteResult("Akumulator double", $"\n\n{resultDouble}");
        }

        private static void TestOpenTextFile()
        {
            var thread = new Thread(() =>
            {
                var fileNames = WinFormsUtils.OpenFiles(requiredFilesCount: 4);

                if (fileNames != null)
                    ConsoleUtils.WriteLines(fileNames);

                var result = WinFormsUtils.OpenTextFile();

                ConsoleUtils.WriteResult("Zawartość otworzonego pliku", result?.Item1);
            });

            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA

            thread.Start();

            thread.Join(); //Wait for the thread to end
        }

        private static void TestOrderBy()
        {
            string[] a = new string[] { "Indonesian", "Korean", "Japanese", "English", "German" };

            var sort = from s in a orderby s select s;

            ConsoleUtils.WriteLines(sort);


            string[] str = new string[] { "abc", "bacd", "pacds" };

            var result = str.Select(c => string.Concat(c.OrderBy(d => d)));

            ConsoleUtils.WriteLines(result);


            var i = new object[] { "efwef", 1, 2.0, true };

            ConsoleUtils.WriteLines(i);


            IEnumerable<object> i2 = new object[] { "efwef", 1, 2.0, true };

            ConsoleUtils.WriteLines(i2);

            ConsoleUtils.WriteLines(1, 2, 3);

            ConsoleUtils.WriteLines(1, false, "ergerg", 2.1, 3m, 6L);
        }

        private static void TestCsvParsing()
        {
            const string pathFormat = "Csv\\{0}.csv";

            string path = string.Format(pathFormat, "fuel");

            ConsoleUtils.WriteTableFromCSV(path);


            path = string.Format(pathFormat, "manufacturers");

            var dt = CSVUtils.ParseCSVToDataTable(path);

            ConsoleUtils.WriteDataTable(dt);

            if (path.EndsWith("fuel.csv"))
                return;

            path = string.Format(pathFormat, "customers");

            var customers = CSVUtils.ParseCSV<Customer, CustomerMap>(path);

            ConsoleUtils.WriteLine(customers.Last().Name);


            var customers2 = CSVUtils.ParseCSV<Customer>(path);

            ConsoleUtils.WriteLine(customers2.First().Name);
        }

        private static void TestGetEnumValues()
        {
            var enumValues = EnumUtils.GetEnumValues(typeof(CertificateType));

            var result = enumValues.ConvertOrCastTo<object, string>();


            string enumName = EnumUtils.GetEnumName<CertificateType>(2);

            result = result.Append(enumName);


            ConsoleUtils.WriteLines(result);



            ConsoleUtils.WriteSeparator();



            var enumValues2 = EnumUtils.GetEnumValues<CertificateType, decimal>();

            ConsoleUtils.WriteLines(enumValues2);


            var certificateType = CertificateType.WSS;

            object enumNumber = Convert.ChangeType(certificateType, typeof(string));

            ConsoleUtils.WriteLine(enumNumber);


            var enumNumbers = EnumUtils.GetEnumNumbers<HorizontalTextAlignment>();

            ConsoleUtils.WriteLines(enumNumbers);
        }

        private static void TestLoggerFactory()
        {
            var loggerFactory = new LoggerFactory();


            loggerFactory.ExecuteCreation(Loggers.Console).Log("Wiadomość", LogType.Fatal);

            loggerFactory.ExecuteCreation(Loggers.Trace).Log("Wiadomość2", LogType.Debug);


            ILogger inMemoryLogger = loggerFactory.ExecuteCreation(Loggers.InMemory);

            inMemoryLogger.Log("Wiadomość3", LogType.Info);
            inMemoryLogger.Log("Wiadomość3", LogType.Error);

            ConsoleUtils.WriteLines(InMemoryLogger.Logs);
        }

        private static void TestFactory()
        {
            AirConditioner.InitializeFactories().ExecuteCreation(Actions.Cooling, 22.5).Operate();
        }

        private static void TestLogMessageFactory()
        {
            LogMessageFactory logMessageFactory = new LogMessageFactory();

            string message = logMessageFactory.Create("Wiadomość", new ArgumentException("Nieprawidłowa wartość argumentu"), LogType.Info);

            string message2 = logMessageFactory.Create(null, new ArgumentException("Nieprawidłowa wartość argumentu"), LogType.Info);

            string message3 = logMessageFactory.Create(null, null, LogType.Info);

            string message4 = logMessageFactory.Create("Wiadomość", null, LogType.Info);

            ConsoleUtils.WriteLines(message, message2, message3, message4);
        }

        private static void TestNameOf()
        {
            NamedObject namedObject = new NamedObject("Nazwa");

            string propertyName = namedObject.GetNameOf(x => x.Name);
            string propertyName2 = namedObject.Name.GetName(() => namedObject.Name);

            ConsoleUtils.WriteResult("Nazwa zmiennej Name z poziomu obiektu", propertyName);
            ConsoleUtils.WriteResult("Nazwa zmiennej Name bezpośrednio", propertyName2);
        }

        private static void TestCategoryCollection()
        {
            Stopwatch stopwatch = new Stopwatch();

            long time1 = stopwatch.TimeMillis(() =>
            {
                CategoryCollection categoryCollection = new CategoryCollection();

                Enumerable.Range(0, 1000000).ForEach(x => categoryCollection.Add("Kategoria", new NamedObject("Nazwa")));
            });

            long time2 = stopwatch.TimeMillis(() =>
            {
                CategoryCollection categoryCollection = new CategoryCollection();

                var itemsToAdd = Enumerable.Range(0, 1000000).Select(x => new NamedObject("Nazwa"));

                categoryCollection.AddRange("Kategoria", itemsToAdd);
            });

            ConsoleUtils.WriteResult("Elementy dodane pojedynczo", time1);
            ConsoleUtils.WriteResult("Elementy dodane wszystkie naraz", time2);
        }

        private static void TestConvertCSVToDataTableAndWriteDataTable()
        {
            string filePath = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 2\Big Data\Laboratoria\Koronawirus - Zadanie\CSVs\Podzielone pliki\20.csv";

            string filePath2 = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 1\Python\Notebooki\Dane\orders\orders.csv";

            string filePath3 = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 2\Data Mining\Ćwiczenia\Dane\datasets_12603_17232_Life Expectancy Data.csv";


            DataTable dataTable = CSVUtils.ParseCSVToDataTable(filePath);

            ConsoleUtils.WriteDataTable(dataTable);


            dataTable = CSVUtils.ParseCSVToDataTable(filePath2, delimiter: "\t");

            ConsoleUtils.WriteDataTable(dataTable);


            dataTable = CSVUtils.ParseCSVToDataTable(filePath3);

            ConsoleUtils.WriteDataTable(dataTable);
        }

        private static void WriteSeparator()
        {
            ConsoleUtils.WriteSeparator(true);
        }

        private static void TestParseCSVToObjectAndDisplayObject()
        {
            const string filePathFormat = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 2\Business Intelligence w przedsięborstwie\Laboratoria\Zadanie 3\Zadanie\Dane\{0}_DATA_TABLE.csv";

            string tableName = "klient";

            string filePath = string.Format(filePathFormat, tableName.ToUpper());


            var customers = CSVUtils.ParseCSV<Customer>(filePath);

            var customersFromMap = CSVUtils.ParseCSV<Customer, CustomerMap>(filePath, false);


            ConsoleUtils.WriteTableFromObjects(customers, false);

            ConsoleUtils.WriteTableFromObjects(customersFromMap, false);
        }

        private static void TestWriteResultAndWriteByte()
        {
            ConsoleUtils.WriteResult("Ilość lat", 23);

            ConsoleUtils.WriteByte(37);
        }

        private static void TestStopWatchTime()
        {
            string filePath = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 1\Python\Notebooki\Dane\orders\orders.csv";

            Stopwatch stopwatch = new Stopwatch();

            long time = stopwatch.TimeMillis(() =>
            {
                CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList();
                CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList();
                CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList();
            });

            Console.WriteLine(time);


            time = ObjectUtils.Time(() => CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList());

            Console.WriteLine(time);


            time = ObjectUtils.Time(() => PDCore.Common.Utils.CSVUtils.ParseCSVLines2(filePath, delimiter: "\t").ToList());

            Console.WriteLine(time);
        }

        private static void TestDisposableStopwatch()
        {
            string filePath = @"C:\Users\pawek\OneDrive\Magisterka\Semestr 1\Python\Notebooki\Dane\orders\orders.csv";

            using (new DisposableStopwatch(t => Console.WriteLine("{0} elapsed", t)))
            {
                // do stuff that I want to measure
                CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList();
            }

            using (new DisposableStopwatch())
            {
                CSVUtils.ParseCSVLines(filePath, delimiter: "\t").ToList();
            }
        }

        private static void TestCacheService()
        {
            ICacheService inMemoryCache = new CacheService();

            IEnumerable<string> lines = ConsoleUtils.ReadLines();


            string text = inMemoryCache.GetOrSet("text", () => Console.ReadLine());
            Console.WriteLine(text);

            text = inMemoryCache.GetOrSet("text", () => Console.ReadLine());
            Console.WriteLine(text);

            text = inMemoryCache.GetOrSet("text", () => Console.ReadLine());
            Console.WriteLine(text);

            lines.Where(x => x.Length > 1).Skip(3).ForEach(x => Console.WriteLine(x));
        }

        private static void TestObjectConvertTo()
        {
            double x = 1.5;

            x = x.ConvertOrCastTo<double, int>();

            Console.WriteLine(x);
        }

        private static void TestIEnumerableConvertTo()
        {
            IEnumerable<double> x = new[] { 1.5, 6.3, 7.9, 5.6 };

            var y = x.ConvertOrCastTo<double, int>();

            string result = string.Join(", ", y);

            Console.WriteLine(result);
        }

        private static void TestSampledAverage()
        {
            var items = new[] { true };

            var result = items.SampledAverage();

            Console.WriteLine(result);
        }

        private static void TestMultiply()
        {
            var item = true;

            int multiplier = 3;

            var result = item.Multiply(multiplier);

            Console.WriteLine(result);
        }

        private static void TestIronPython()
        {
            var pythonScript = new PythonScript();


            var result = pythonScript.RunFromFile<string>("PythonScriptSimple.py", "z");

            Console.WriteLine(result);


            var result2 = pythonScript.RunFromFile<IEnumerable<string>>("PythonScriptSimple2.py", "cars");

            ConsoleUtils.WriteLines(result2);
        }

        private static void TestPython()
        {
            var result = PythonUtils.Run("PythonScript.py");

            Console.WriteLine(result);
        }

        private static void TestPythonNet()
        {
            string pythonHomePath = Path.Combine("C" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar, "Python39");

            Environment.SetEnvironmentVariable("PYTHONHOME", pythonHomePath, EnvironmentVariableTarget.Process);

            Runtime.PythonDLL = pythonHomePath + Path.DirectorySeparatorChar + "python39.dll";

            using (Py.GIL())
            {           
                using (PyScope scope = Py.CreateScope())
                {
                    scope.Set("canPrint", false);


                    string code = File.ReadAllText("PythonScript.py");

                    scope.Exec(code);

                    var result = scope.Get<double[]>("r");

                    ConsoleUtils.WriteLines(result);


                    code = File.ReadAllText("PythonScript2.py");

                    scope.Exec(code);

                    var result2 = scope.Get<double[][]>("a");

                    ConsoleUtils.WriteTableFromFields(result2, false);


                    var rg = scope.Eval("np.random.default_rng(1)");
                    
                    scope.Set("rg", rg);


                    var result3 = scope.Eval<double>("rg.random((2,3)).sum()");

                    Console.WriteLine(result3);
                }
            }
        }
    }
}
