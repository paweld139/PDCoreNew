using PDCore.Utils;
using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace PDCore.Extensions
{
    /// <summary>
    /// Statyczna klasa rozszerzająca pomocn przy operacjach wejścia-wyjścia
    /// </summary>
    public static class IOExtension
    {
        /// <summary>
        /// Zwrócenie tablicy bajtów ze strumienia
        /// </summary>
        /// <param name="input">Strumień</param>
        /// <returns>Tablica bajtów ze strumienia</returns>
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024]; //Utworzenie tablicy bajtów o pojemności ok. 16 KB

            using (MemoryStream ms = new MemoryStream()) //Utworzenie strumienia pamięci
            {
                int read; //Utworzenie zmiennej, która przechowa ilość bajtów wyczytanych ze strumienia

                //Odczytanie bajtów ze strumienia do bufora maksymalnie o wielkości bufora, bez pomijania bajtów, dopóki pozostały bajty do odczytania.
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read); //Zapisanie do strumienia pamięci, bajtów przechowywanych przez bufor
                }

                return ms.ToArray(); //Zwrócenie tablicy bajtów z bufora pamięci
            }
        }

        public static void OpenConnectionIfClosed(this DbConnection dbConnection)
        {
            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();
        }

        public static Task OpenConnectionIfClosedAsync(this DbConnection dbConnection)
        {
            Task task = Task.CompletedTask;

            if (dbConnection.State != ConnectionState.Open)
                task = dbConnection.OpenAsync();

            return task;
        }

        /// <summary>
        /// Zwraca tablicę bajtów dla zadanego obiektu Image i formatu zdjęcia
        /// </summary>
        /// <param name="image">Zdjęcea</param>
        /// <param name="imageFormat">Format zdjęcia</param>
        /// <returns>Tablica bajtów będąca odzwierciedleniem przekazanego zdjęcia i biorąca pod uwagę format</returns>
        public static byte[] GetBuffer(this Image image, ImageFormat imageFormat)
        {
            using (MemoryStream ms = new MemoryStream()) //Utworzenie strumienia danych
            {
                image.Save(ms, imageFormat); //Zapisuje zdjęcie w zadanym formacie do strumienia danych

                byte[] buf = ms.GetBuffer(); //Pobranie tablicy bajtów ze strumienia danych

                return buf; //Zwrócenie tablicy najtów
            }
        }
    }
}
