using CsvHelper;
using CsvHelper.Configuration;
using PDCore.Extensions;
using PDCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace PDCore.Utils
{
    public static class CSVUtils
    {
        #region CSV lines parsing

        public static IEnumerable<string[]> ParseCSVLines(string filePath, bool skipFirstLine = false, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null)
        {
            using (var csvReader = GetCsvReader(filePath, skipFirstLine, delimiter, shouldSkipRecord))
            {
                while (csvReader.Read())
                {
                    yield return csvReader.CurrentRecord;
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
        public static IEnumerable<T> ParseCSV<T>(string filePath, Func<string[], T> fieldsParser, bool skipFirstLine = true, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null)
        {
            List<string[]> linesFields = ParseCSVLines(filePath, skipFirstLine, delimiter, shouldSkipRecord).ToList(); //Utworzenie kolekcji pól dla każdej linii pliku CSV, wybór linii następuje wg wskazanych warunków.

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
        public static IEnumerable<T> ParseCSV<T>(string filePath, bool skipFirstLine = true, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null) where T : IFromCSVParseable, new() //Typ musi posiadać konstruktor
        {
            return ParseCSV(
                filePath,
                x =>
                {
                    var t = new T();
                    t.ParseFromCSV(x);
                    return t;
                }, skipFirstLine, delimiter, shouldSkipRecord); //Przetworzenie pliku na kolekcję obiektów. Przekazano metodę do przetwarzania pól z danej linii na obiekt. Wybór linii następuje wg wskazanych warunków.
        }

        public static List<T> ParseCSV<T, TMap>(string filePath, bool skipFirstLine = true, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null) where TMap : CsvClassMap<T>, new()
        {
            using (var csvReader = GetCsvReader(filePath, skipFirstLine, delimiter, shouldSkipRecord, new TMap()))
            {
                return csvReader.GetRecords<T>().ToList();
            }
        }

        public static DataTable ParseCSVToDataTable(string filePath, bool hasHeader = true, bool skipFirstLine = false, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null)
        {
            var dt = new DataTable();

            dt.WriteCsv(filePath, hasHeader, skipFirstLine, delimiter, shouldSkipRecord);

            return dt;
        }

        public static CsvReader GetCsvReader(string filePath, bool skipFirstLine = true, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null, CsvClassMap csvClassMap = null)
        {
            CsvReader csvReader = new CsvReader(
                File.OpenText(filePath),
                new CsvConfiguration
                {
                    Delimiter = delimiter,
                    HasHeaderRecord = skipFirstLine,
                    SkipEmptyRecords = true,
                    ShouldSkipRecord = shouldSkipRecord,
                    IgnoreBlankLines = true,
                    DetectColumnCountChanges = true,
                    WillThrowOnMissingField = true
                });

            if (csvClassMap != null)
                csvReader.Configuration.RegisterClassMap(csvClassMap);

            return csvReader;
        }

        #endregion
    }
}
