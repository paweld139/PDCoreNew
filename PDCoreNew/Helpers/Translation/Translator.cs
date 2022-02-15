﻿using PDCoreNew.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PDCoreNew.Helpers.Translation
{
    public abstract class Translator
    {
        protected abstract Dictionary<string, Func<string>> Sentences { get; }

        protected abstract Dictionary<string, Func<string>> Words { get; }


        public bool CanTranslateSentence(string sentence)
        {
            return Sentences.ContainsKey(sentence);
        }

        public bool CanTranslateSentences(IEnumerable<string> sentences)
        {
            return sentences.Any(s => CanTranslateSentence(s));
        }

        public bool CanTranslateWord(string word)
        {
            return Words.ContainsKey(word);
        }

        public bool CanTranslateWords(IEnumerable<string> words)
        {
            return words.Any(s => CanTranslateWord(s));
        }


        public bool TranslateText(ref string text)
        {
            bool result = CanTranslateSentence(text);

            if (result)
            {
                TranslateSentence(ref text, true);
            }
            else
            {
                var sentences = text.GetSentences().ToList();

                var translatedSentences = new List<string>(sentences);

                result = TranslateSentences(translatedSentences);

                if (result)
                {
                    StringBuilder translatedText = new(text);

                    sentences.ForEach((s, i) => translatedText.Replace(s, translatedSentences[i]));

                    text = translatedText.ToString();
                }
            }

            return result;
        }


        public bool TranslateWord(ref string word, bool force = false)
        {
            bool result = force || CanTranslateWord(word);

            if (result)
            {
                word = Words[word]();
            }

            return result;
        }

        public bool TranslateWords(IList<string> words, bool force = false)
        {
            bool result = force || CanTranslateWords(words);

            if (result)
            {
                string word;

                for (int i = 0; i < words.Count; i++)
                {
                    word = words[i];

                    if (CanTranslateWord(word))
                    {
                        TranslateWord(ref word, true);

                        words[i] = word;
                    }
                }
            }

            return result;
        }

        public bool TranslateSentence(ref string sentence, bool force = false)
        {
            bool result = force || CanTranslateSentence(sentence);

            if (result)
            {
                sentence = Sentences[sentence]();
            }
            else
            {
                List<string> words = sentence.GetWords().ToList();

                result = CanTranslateWords(words);

                if (result)
                {
                    List<string> translatedWords = new(words);

                    TranslateWords(translatedWords, true);


                    StringBuilder translatedSentence = new(sentence);

                    words.ForEach((w, i) => translatedSentence.Replace(w, translatedWords[i]));

                    sentence = translatedSentence.ToString();
                }
            }

            return result;
        }

        public string TranslateWord(string word, bool force = false)
        {
            TranslateWord(ref word, force);

            return word;
        }

        public string TranslateSentence(string sentence, bool force = false)
        {
            TranslateSentence(ref sentence, force);

            return sentence;
        }

        public string TranslateText(string text)
        {
            TranslateText(ref text);

            return text;
        }

        public bool TranslateSentences(IList<string> sentences)
        {
            bool result = false;

            string sentence;

            for (int i = 0; i < sentences.Count; i++)
            {
                sentence = sentences[i];

                if (TranslateSentence(ref sentence))
                {
                    sentences[i] = sentence;

                    if (!result)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }


        public void Translate(DataRow dr, string columnName)
        {
            string text = dr[columnName].ToString();

            bool result = TranslateText(ref text);

            if (result)
            {
                dr.SetField(columnName, text);
            }
        }

        public void Translate(DataRow dr, string[] columnNames)
        {
            columnNames.ForEach(x => Translate(dr, x));
        }


        public void Translate(DataTable dt, string columnName)
        {
            foreach (DataRow item in dt.Rows)
            {
                Translate(item, columnName);
            }
        }

        public void Translate(DataTable dt, string[] columnNames)
        {
            foreach (DataRow item in dt.Rows)
            {
                Translate(item, columnNames);
            }
        }
    }
}
