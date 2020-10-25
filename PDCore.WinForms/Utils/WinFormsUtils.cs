﻿using PDCore.Extensions;
using PDCore.Helpers.Calculation;
using PDCore.WinForms.Extensions;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PDCore.WinForms.Utils
{
    public static class WinFormsUtils
    {
        public static int[] GetRandomNumbers(NumericUpDown fromNumericUpDown, NumericUpDown toNumericUpDown, NumericUpDown amountNumericUpDown)
        {
            int from = fromNumericUpDown.GetValueInt();

            int to = toNumericUpDown.GetValueInt();

            int amount = amountNumericUpDown.GetValueInt();


            int[] result = null;

            try
            {
                result = RandomNumberGenerator.Next(from, to, amount);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ShowError(ex);
            }

            return result;
        }

        public static void SetMinAndMaxAsInt(params NumericUpDown[] numericUpDown)
        {
            numericUpDown.ForEach(x => x.SetMinAndMaxAsInt());
        }

        public static void ShowMessage(string content, string title)
        {
            MessageBox.Show(content, title);
        }

        public static void ShowError(string content)
        {
            ShowMessage(content, "Uwaga");
        }

        public static void ShowInformation(string content)
        {
            ShowMessage(content, "Informacja");
        }

        public static void ShowError(string content, Exception exception)
        {
            string message = string.Format("{0}{1}{1}{2}{1}{1}{3}", content, Environment.NewLine, exception.Message, exception.StackTrace);

            ShowError(message);
        }

        public static void ShowError(Exception exception)
        {
            ShowError(exception.Message);
        }

        public static bool ShowQuestion(string content, string title = "Uwaga")
        {
            DialogResult dialogResult = MessageBox.Show(content, title, MessageBoxButtons.YesNo);

            bool approved = dialogResult == DialogResult.Yes;

            return approved;
        }

        public static string[] OpenFiles(string title = "Otwórz", string filter = null, int requiredFilesCount = 0)
        {
            if (requiredFilesCount < 0)
                throw new ArgumentOutOfRangeException(nameof(requiredFilesCount), requiredFilesCount, "Podano nieprawidłową ilość plików do wybrania. Minimum to 0 (dowolna ilość).");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;

                openFileDialog.Title = title; //"Otwórz pliki do importu (rejestr RPL i SORL)";

                openFileDialog.Filter = filter; //"XML Files|*.xml";

                openFileDialog.Multiselect = requiredFilesCount != 1; //true;


                if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
                {
                    int selectedFilesCount = openFileDialog.FileNames.Length;

                    if (requiredFilesCount.ValueIn(selectedFilesCount, 0))
                    {
                        return openFileDialog.FileNames;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Oczekiwano wyboru {requiredFilesCount} plików, a wybrano {selectedFilesCount}.");
                    }
                }

                return null;
            }
        }

        public static Tuple<string, string> OpenTextFile()
        {
            string[] fileNames;

            try
            {
                fileNames = OpenFiles("Otwórz plik tekstowy", "Text |*.txt", 1);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentOutOfRangeException)
                {
                    ShowError(ex);
                }

                throw;
            }

            if (fileNames != null) //Wybrano pliki i nie wystąpił wyjątek
            {
                string fileName = fileNames[0];

                string text = File.ReadAllText(fileName, Encoding.UTF8);


                return Tuple.Create(text, fileName);
            }

            return null;
        }
    }
}
