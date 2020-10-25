using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PDCore.Common.Utils
{
    public static class CSVUtils
    {
        /// <summary>
        /// Zwrócenie listy pól dla zadanej linii pliku CSV
        /// </summary>
        /// <param name="lineContent">Zawartość danej linii</param>
        /// <param name="delimiter">Znak oddzielający dane w liniach pliku CSV</param>
        /// <returns>Tablica pól dla zadanej linii pliku CSV</returns>
        public static string[] ParseCSVLine(string lineContent, string delimiter = ",")
        {
            using (StringReader stringReader = new StringReader(lineContent)) //Odczytanie danych linii
            {
                using (TextFieldParser textFieldParser = new TextFieldParser(stringReader)) //Utworzenie instancji klasy przetwarzającej linie pliku CSV na tablicę pól
                {
                    textFieldParser.SetDelimiters(delimiter); //Ustawienie znaku oddzielającego dane w liniach pliku CSV

                    return textFieldParser.ReadFields(); //Odczytanie pól z aktualnej linii pliku CSV. Przekazana zostaje tylko jedna linia, więc tylko jedna zostanie przetworzona i zwrócona

                    //Po zwrócenia tablicy pól, następuje zniszczenie instancji klas
                }
            }
        }

        /// <summary>
        /// Zwrócenie kolekcji pól dla wybranych linii pliku CSV
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku CSV</param>
        /// <param name="skipFirstLine">Czy pominąć pierwszą linię pliku CSV, zazwyczaj jest to nagłówek, a nie zawsze jest potrzebny</param>
        /// <param name="delimiter">Znak oddzielający dane w liniach pliku CSV</param>
        /// <param name="shouldSkipRecord">Warunek. który musi spełnić dana linia, by została wzięta pod uwagę</param>
        /// <returns>Kolekcja pól dla wybranych linii pliku CSV</returns>
        public static IEnumerable<string[]> ParseCSVLines2(string filePath, bool skipFirstLine = false, string delimiter = ",", Func<string[], bool> shouldSkipRecord = null)
        {
            IEnumerable<string> lines = File.ReadLines(filePath).Where(x => x.Length > 1); //Odroczone odczytanie linii z pliku CSV, których ilość znaków jest większa od 1

            if (skipFirstLine) //Czy pominąć pierwszą linię
                lines = lines.Skip(1); //Następuje pominięcie pierwszej linii

            IEnumerable<string[]> linesFields = lines.Select(x => ParseCSVLine(x, delimiter)); //Dla każdej wybranej linii otrzymana zostaje tabica pól i powstaje kolekcja

            if (shouldSkipRecord != null)
                return linesFields.Where(x => !shouldSkipRecord(x)); //Zwrócenie kolekcji pól dla linii

            return linesFields;
        }
    }
}
