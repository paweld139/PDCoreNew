using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PDCore.Extensions;
using PDCore.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PDCore.Utils
{
    public static class CSVUtils
    {
        #region CSV lines parsing

        public static IEnumerable<string[]> ParseCSVLines(string filePath, Encoding encoding = null, CultureInfo cultureInfo = null, bool skipFirstLine = false,
            string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null)
        {
            using (var csvReader = GetCsvReader(filePath, encoding, cultureInfo, skipFirstLine, delimiter, shouldSkipRecord))
            {
                while (csvReader.Read())
                {
                    yield return csvReader.GetRecords<string>().ToArray();
                }
            }
        }

        #endregion


        #region CSV parsing

        /// <summary>
        /// Zwrócenie listy obiektów na podstawie pliku CSV
        /// </summary>
        /// <typeparam name="T">Typ na który ma być przetworzona linia w pliku CSV</typeparam>
        /// <param name="filePath">Ścieżka do pliku CSV</param>
        /// <param name="fieldsParser">Metoda do przetworzenia tablicy pól w danej linii pliku CSV na obiekt, którego kolekcja zostanie zwrócona</param>
        /// <param name="skipFirstLine">Czy pominąć pierwszą linię pliku CSV, zazwyczaj jest to nagłówek, a nie zawsze jest potrzebny</param>
        /// <param name="delimiter">Znak oddzielający dane w liniach pliku CSV</param>
        /// <param name="shouldSkipRecord">Warunek. który musi spełnić dana linia, by została wzięta pod uwagę</param>
        /// <returns>Lista obiektów z przetworzonego pliku CSV</returns>
        public static IEnumerable<T> ParseCSV<T>(string filePath, Func<string[], T> fieldsParser, Encoding encoding = null, CultureInfo cultureInfo = null,
            bool skipFirstLine = true, string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null)
        {
            List<string[]> linesFields = ParseCSVLines(filePath, encoding, cultureInfo, skipFirstLine, delimiter, shouldSkipRecord).ToList(); //Utworzenie kolekcji pól dla każdej linii pliku CSV, wybór linii następuje wg wskazanych warunków.

            return linesFields.Select(x => fieldsParser(x)); //Przetworzenie pól z każdej linii i zwrócenie otrzymanej kolekcji obiektów
        }

        /// <summary>
        /// Zwrócenie listy obiektów na podstawie pliku CSV
        /// </summary>
        /// <typeparam name="T">Typ na który ma być przetworzona linia w pliku CSV. Musi implementować interfejs IFromCSVParseable, czyli posiadać metodę do ustawienia danych na podstawie pól danej linii</typeparam>
        /// <param name="filePath">Ścieżka do pliku CSV</param>
        /// <param name="skipFirstLine">Czy pominąć pierwszą linię pliku CSV, zazwyczaj jest to nagłówek, a nie zawsze jest potrzebny</param>
        /// <param name="delimiter">Znak oddzielający dane w liniach pliku CSV</param>
        /// <param name="shouldSkipRecord">Warunek. który musi spełnić dana linia, by została wzięta pod uwagę</param>
        /// <returns>Lista obiektów z przetworzonego pliku CSV</returns>
        public static IEnumerable<T> ParseCSV<T>(string filePath, Encoding encoding = null, CultureInfo cultureInfo = null, bool skipFirstLine = true,
            string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null) where T : IFromCSVParseable, new() //Typ musi posiadać konstruktor
        {
            return ParseCSV(
                filePath,
                x =>
                {
                    var t = new T();
                    t.ParseFromCSV(x);
                    return t;
                }, 
                encoding,
                cultureInfo,
                skipFirstLine, delimiter, shouldSkipRecord); //Przetworzenie pliku na kolekcję obiektów. Przekazano metodę do przetwarzania pól z danej linii na obiekt. Wybór linii następuje wg wskazanych warunków.
        }

        public static List<T> ParseCSV<T, TMap>(string filePath, bool skipFirstLine = true, Encoding encoding = null, CultureInfo cultureInfo = null,
            string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null) where TMap : ClassMap<T>, new()
        {
            using (var csvReader = GetCsvReader(filePath, encoding, cultureInfo, skipFirstLine, delimiter, shouldSkipRecord, new TMap()))
            {
                return csvReader.GetRecords<T>().ToList();
            }
        }

        public static DataTable ParseCSVToDataTable(string filePath, Encoding encoding = null, CultureInfo cultureInfo = null, bool hasHeader = true, 
            bool skipFirstLine = false, string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null)
        {
            var dt = new DataTable();

            dt.WriteCsv(filePath, encoding, cultureInfo, hasHeader, skipFirstLine, delimiter, shouldSkipRecord);

            return dt;
        }

        public static CsvReader GetCsvReader(string filePath, Encoding encoding, CultureInfo cultureInfo, bool skipFirstLine = true,
            string delimiter = ",", ShouldSkipRecord shouldSkipRecord = null, ClassMap csvClassMap = null)
        {
            CsvReader csvReader = new CsvReader(
                File.OpenText(filePath),
                new CsvConfiguration(cultureInfo ?? CultureInfo.InvariantCulture)
                {
                    Delimiter = delimiter,
                    HasHeaderRecord = skipFirstLine,
                    ShouldSkipRecord = shouldSkipRecord,
                    IgnoreBlankLines = true,
                    DetectColumnCountChanges = true,
                    MissingFieldFound = null,
                    Encoding = encoding ?? Encoding.UTF8
                });

            if (csvClassMap != null)
                csvReader.Context.RegisterClassMap(csvClassMap);

            return csvReader;
        }


        #region Writer

        public static CsvWriter GetCsvWriter(TextWriter textWriter, Encoding encoding, CultureInfo cultureInfo,
           bool hasHeaderRecord = true, string delimiter = ",", bool isoDateTime = true)
        {
            CsvWriter csvWriter = new CsvWriter(
                textWriter,
                new CsvConfiguration(cultureInfo)
                {
                    Delimiter = delimiter,
                    HasHeaderRecord = hasHeaderRecord,
                    ShouldSkipRecord = record => record.Record.All(string.IsNullOrEmpty),
                    IgnoreBlankLines = true,
                    DetectColumnCountChanges = true,
                    MissingFieldFound = null,
                    Encoding = encoding
                });

            if (isoDateTime)
            {
                var options = new TypeConverterOptions
                {
                    Formats = new[] { "o" }
                };

                //apply options to datetime
                csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
            }

            return csvWriter;
        }

        public static CsvWriter GetCsvWriter(string filePath, Encoding encoding, CultureInfo cultureInfo,
            bool hasHeaderRecord = true, string delimiter = ",", bool isoDateTime = true)
        {
            return GetCsvWriter(File.CreateText(filePath), encoding, cultureInfo, hasHeaderRecord, delimiter, isoDateTime);
        }

        public static string GetCSV(IEnumerable records, TextWriter textWriter, Encoding encoding, CultureInfo cultureInfo,
            bool hasHeaderRecord = true, string delimiter = ",", bool isoDateTime = true)
        {
            using (var csvWriter = GetCsvWriter(textWriter, encoding, cultureInfo, hasHeaderRecord, delimiter, isoDateTime))
            {
                csvWriter.WriteRecords(records);

                return textWriter.ToString();
            }
        }

        public static string GetCSV(IEnumerable records, Encoding encoding, CultureInfo cultureInfo,
            bool hasHeaderRecord = true, string delimiter = ",", bool isoDateTime = true)
        {
            using (var stringWriter = new StringWriter())
                return GetCSV(records, stringWriter, encoding, cultureInfo, hasHeaderRecord, delimiter, isoDateTime);
        }

        #endregion


        #endregion
    }
}
