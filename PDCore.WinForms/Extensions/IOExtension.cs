using PDCore.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PDCore.WinForms.Extensions
{
    /// <summary>
    /// Statyczna klasa rozszerzająca pomocn przy operacjach wejścia-wyjścia
    /// </summary>
    public static class IOExtension
    {
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

        /// <summary>
        /// Zapisanie zdjęcia o zadanym formacie i nazwie jako pliku tymczasowego
        /// </summary>
        /// <param name="image">Zdjęcie do zapisania</param>
        /// <param name="imageFormat">Format w jakim zdjęcie zostanie zapisane</param>
        /// <param name="name">Nazwa pliku ze zdjęciem do zapisania</param>
        /// <returns>Ścieżka do zapisanego zdjęcia</returns>
        public static string SaveTemp(this Image image, ImageFormat imageFormat, string name)
        {
            string extension = new ImageFormatConverter().ConvertToString(imageFormat); //Przekonwertowanie formatu zdjęcia na rozszerzenie w formie łańcucha znaków

            string fileName = name + "." + extension; //Utworzenie nazwy pliku na podstawie przekazanej nazwy i otrzymanego roszerzenia

            string path = SecurityUtils.TemplateDirPath() + fileName; //Utworzenie ścieżki w jakiej zostanie zapisany plik, na podstawie pobranej ścieżki plików tymczasowych i otrzymanej nazwy pliku

            File.WriteAllBytes(path, image.GetBuffer(imageFormat)); //Pobranie tablicy bajtów dla zadanego zdjęcia i zapisanie zdjęcia w zadanej lokalizacji

            return path; //Zwrócenie ścieżki do zdjęcia
        }
    }
}
