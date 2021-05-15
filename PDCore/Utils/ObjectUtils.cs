using PDCore.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace PDCore.Utils
{
    public static class ObjectUtils
    {
        public static string GetParam(object param)
        {
            if (param == null)
            {
                return string.Empty;
            }
            else
            {
                switch (param)
                {
                    case var p when p is string:
                        return string.Format("\"{0}\"", (string)param);

                    case var p when p is decimal:
                        return ((decimal)param).ToString(CultureInfo.InvariantCulture);

                    case var p when p is bool:
                        return Convert.ToByte(param).ToString();

                    case var p when p is DateTime:
                        return ((DateTime)param).GetLong().ToString();

                    case var p when p is Enum:
                        return ((Enum)param).ToString("D");
                }

                return param.ToString();
            }
        }

        public static string GetContent(params object[] content)
        {
            string[] parameters = new string[content.Length];

            int index = 0;

            foreach (object item in content)
            {
                parameters[index] = GetParam(item);

                index++;
            }

            return string.Join(",", parameters);
        }

        public static string Validate(object o)
        {
            ValidationContext context = new ValidationContext(o, null, null);

            IList<ValidationResult> errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(o, context, errors, true))
            {
                return string.Join(Environment.NewLine, errors);
            }
            else
            {
                return null;
            }
        }
        public static bool AreNull(params object[] results)
        {
            return AreOrNotNull(true, results);
        }

        public static bool AreNotNull(params object[] results)
        {
            return AreOrNotNull(false, results);
        }

        private static bool AreOrNotNull(bool indicator = false, params object[] results)
        {
            bool result = true;

            foreach (object item in results)
            {
                result &= ((item == null) == indicator);

                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        public static string FirstNotNullOrWhiteSpace(params string[] results)
        {
            return results.First(x => !string.IsNullOrWhiteSpace(x));
        }

        public static void ThrowIfNull(params object[] objects) //Parametry mogą mieć różne typy i dlatego brak parametru generycznego
        {
            objects.ForEach(x => x.ThrowIfNull());
        }

        public static void ThrowIfNull(params Tuple<string, object>[] objects)
        {
            objects.ForEach(x => x.Item2.ThrowIfNull(x.Item1));
        }

        public static void SwapValues<T>(ref T object1, ref T object2) //Zmienne muszą być zainicjalizowane przed przekazaniem do metody. Można odczytać wartości.
        {
            T temp = object1;


            object1 = object2;

            object2 = temp;
        }

        public static long Time(Action action, int iterations = 1)
        {
            Stopwatch stopwatch = new Stopwatch();

            return stopwatch.TimeMillis(action, iterations);
        }

        public static IEnumerable<double> Random()
        {
            var random = new Random();

            while (true)
            {
                yield return random.NextDouble(); //Między 0 a 1
            }
        }

        public static IEnumerable<int> GetRandomNumbers(int maxValue)
        {
            Random rand = new Random();

            while (true)
            {
                yield return rand.Next(maxValue);
            }
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> GetKVPs<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            return keys.GetKVP(k => k, i => values.ElementAt(i));
        }

        public static IEnumerable<T> GetChangedObjects<T>(IEnumerable<T> objects, IEnumerable<T> cachedObjects)
        {
            var newAndChanged = objects.Except(cachedObjects);
            var removedAndChanged = cachedObjects.Except(objects);
            var changed = newAndChanged.Concat(removedAndChanged);

            return changed;
        }

        public static IList<int> FindLargePrimes(int start, int end)
        {
            var primes = Enumerable.Range(start, end - start).ToList();

            return primes.Where(NumberExtension.IsPrime).ToList();
        }

        public static IList<int> FindLargePrimesInParallel(int start, int end)
        {
            var primes = Enumerable.Range(start, end - start).ToList();

            return primes.AsParallel().Where(n => n.IsPrime()).ToList();
        }

        public static Task<Tuple<TOutput, Exception>> GetResultWithRetryAsync<TInput, TInput2, TOutput>(Func<TInput, TInput2, Task<TOutput>> input,
            TInput param, TInput2 param2)
        {
            return input.Partial(param, param2).WithRetry();
        }

        public static Task<Tuple<TOutput, Exception>> GetResultWithRetryAsync<TInput, TInput2, TInput3, TOutput>(
            Func<TInput, TInput2, TInput3, Task<TOutput>> input,
            TInput param, TInput2 param2, TInput3 param3)
        {
            return input.Partial(param, param2, param3).WithRetry();
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);

                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] byteArray)
        {
            if (byteArray == null)
                return null;

            object obj = null;

            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(byteArray, 0, byteArray.Length);

                ms.Seek(0, SeekOrigin.Begin);

                obj = bf.Deserialize(ms);
            }

            return obj;
        }
    }
}
