using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using PDCoreNew.Helpers;
using PDCoreNew.Helpers.Wrappers.DisposableWrapper;
using PDCoreNew.Interfaces;
using PDCoreNew.Models;
using PDCoreNew.Repositories.IRepo;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PDCoreNew.Extensions
{
    public static class ObjectExtension
    {
        // core "just dispose it without barfing"
        public static IDisposableWrapper<T> Wrap<T>(this T baseObject) where T : class, IDisposable
        {
            if (baseObject is IDisposableWrapper<T> wrapper)
            {
                return wrapper;
            }

            return new DisposableWrapper<T>(baseObject);
        }

        //public static void ThrowIfNull(this object obj)
        //{
        //    if (obj == null)
        //        throw new Exception();
        //}

        public static void ThrowIfNull<T>(this T obj, string objName)
        {
            if (obj == null)
                throw new ArgumentNullException(objName);
        }

        public static void ThrowIfNull<T>(this T obj)
        {
            obj.ThrowIfNull(nameof(obj));
        }

        public static void SwapValues<T>(this T[] source, long index1, long index2)
        {
            ObjectUtils.SwapValues(ref source[index1], ref source[index2]);
        }

        public static void SwapValues<T>(this T[] source)
        {
            if (source.Length < 2)
                throw new ArgumentException("Tablica musi zawierać co najmniej dwa elementy");

            source.SwapValues(0, 1);
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf(typeof(T));

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }

        public static Tuple<TimeSpan, TResult> Time<T, TResult>(this Stopwatch sw, Func<T, TResult> func, T param, int iterations = 1)
        {
            sw.Reset();

            sw.Start();

            TResult result = default;

            for (int i = 0; i < iterations; i++)
            {
                result = func(param);
            }

            sw.Stop();

            return new Tuple<TimeSpan, TResult>(sw.Elapsed, result);
        }

        public static Tuple<TimeSpan, T> Time<T>(this Stopwatch sw, Func<T> func, int iterations = 1)
        {
            return sw.Time(f => func(), true, iterations);
        }

        public static long TimeMillis(this Stopwatch sw, Action action, int iterations = 1)
        {
            var elapsed = sw.Time(p => { action(); return true; }, true, iterations);

            return elapsed.Item1.Milliseconds;
        }

        public static IDisposableWrapper<DisposableStopwatch> WrapStopwatch(this DisposableStopwatch disposableStopwatch)
        {
            return new StopWatchDisposableWrapper(disposableStopwatch);
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static TOutput ConvertOrCastTo<TInput, TOutput>(this TInput input, Converter<TInput, TOutput> converter = null)
        {
            if (converter != null)
                return converter(input);

            if (input == null)
                return default;

            if (input is TOutput)
                return input.CastObject<TOutput>();

            //var simpleConverter = TypeDescriptor.GetConverter(typeof(TInput));

            Type type = Nullable.GetUnderlyingType(typeof(TOutput)) ?? typeof(TOutput);

            return (TOutput)input.ConvertObject(type);
        }

        public static bool TryConvertOrCastTo<TInput, TOutput>(this TInput input, out TOutput output, Converter<TInput, TOutput> converter = null)
        {
            try
            {
                output = input.ConvertOrCastTo(converter);

                return true;
            }
            catch
            {
                output = default;
            }

            return false;
        }

        /// <summary>
        /// Pobierz opis dla danej wartości enuma
        /// </summary>
        /// <typeparam name="T">Typ enuma</typeparam>
        /// <param name="enumerationValue">Wartość enuma</param>
        /// <returns>Opis dla danej wartości enuma z atrybutu Description</returns>
        public static string GetDescription<T>(this T enumerationValue) where T : struct //Typ musi być typem wartościowym, enumem
        {
            Type type = enumerationValue.GetType(); //Pobranie typu przekazanej wartości enuma

            if (!type.IsEnum) //Jeśli nie jest to enum
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue)); //Problem z argumentem, więc odpowiedni wyjątek
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString()); //Pobranie wartości enuma jako członka po nazwie jego typu

            if (memberInfo != null && memberInfo.Length > 0) //Jeśli członek istnieje
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false); //Pobranie atrybutu dla cżłonka

                if (attrs != null && attrs.Length > 0) //Jeżeli znaleziono atrybut
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description; //Pobranie wartości z atrybutu
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static T CastObject<T>(this object input)
        {
            return (T)input;
        }

        public static T ConvertObject<T>(this object input)
        {
            return (T)input.ConvertObject(typeof(T));
        }

        public static object ConvertObject(this object input, Type outputType)
        {
            return input.ConvertObject(outputType.GetTypeCode());
        }

        public static object ConvertObject(this object input, TypeCode outputTypeCode)
        {
            return Convert.ChangeType(input, outputTypeCode);
        }

        public static string EmptyIfNull<T>(this T value)
        {
            if (value == null)
                return string.Empty;

            return value.ToString();
        }

        public static Tuple<T1, object> GetTuple<T1>(this object item2, T1 item1)
        {
            return Tuple.Create(item1, item2);
        }

        public static int GetEnumNumber<TEnum>(this TEnum value) where TEnum : struct
        {
            return value.ConvertOrCastTo<TEnum, int>();
        }

        public static string ToNumberString<T>(this T value, int precision, CultureInfo cultureInfo)
        {
            return value.ToString().ToNumberString(precision, cultureInfo);
        }

        public static string ToNumberString<T>(this T value, int precision)
        {
            return ToNumberString(value, precision, CultureInfo.CurrentUICulture);
        }

        public static bool ValueIn<TInput>(this TInput input, params TInput[] values) where TInput : IEquatable<TInput>
        {
            return values.Any(v => v.Equals(input));
        }

        public static bool ArePropertiesNotNull<T>(this T obj)
        {
            if (obj == null)
                return false;

            return PropertyCache<T>.PublicProperties.All(propertyInfo => propertyInfo.GetPropertyValue(obj) != null);
        }

        public static string GetUserName(this SmtpClient smtpClient) => (smtpClient.Credentials as NetworkCredential)?.UserName;

        public static ConsoleColor ToConsoleColor(this Color color)
        {
            return color.Name.ParseEnum<ConsoleColor>();
        }

        public static bool IsNew(this IModificationHistory history) => history.DateCreated == DateTime.MinValue;

        public static RowInfo GetRowInfo(this IModificationHistory modificationHistory) => new(modificationHistory);

        public static EntityInfo<TKey> GetEntityInfo<TKey>(this IEntity<TKey> entity) where TKey : IEquatable<TKey>
        {
            return new EntityInfo<TKey>(entity);
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()?
                            .GetMember(enumValue.ToString())?
                            .First()?
                            .GetAttribute<DisplayAttribute>()?
                            .Name;
        }

        public static string ToLowerString(this bool input) => input.ToString().ToLowerInvariant();

        public static IDisposableWrapper<ISqlRepositoryEntityFramework<TModel>> WrapRepo<TModel>(this ISqlRepositoryEntityFramework<TModel> repo, bool withoutValidation = false) where TModel : class, IModificationHistory
        {
            return new SaveChangesWrapper<TModel>(repo, withoutValidation);
        }

        private async static Task<Tuple<TResult, TException>> DoWithRetry<TResult, TException>(Func<TResult> func, Func<Task<TResult>> task, bool sync) where TException : Exception
        {
            var result = default(TResult);

            TException exception = null;

            int retryCount = 0;

            bool succesful = false;

            do
            {
                try
                {
                    if (sync)
                        result = func();
                    else
                        result = await task();

                    succesful = true;
                }
                catch (TException ex)
                {
                    exception = ex;

                    if (ex is TaskCanceledException)
                        succesful = true;
                    else
                        retryCount++;
                }
            } while (retryCount < 3 && !succesful);

            return Tuple.Create(result, exception);
        }

        public static Tuple<TResult, TException> WithRetry<TResult, TException>(this Func<TResult> func) where TException : Exception
        {
            return DoWithRetry<TResult, TException>(func, null, true).Result;
        }

        public static Task<Tuple<TResult, TException>> WithRetry<TResult, TException>(this Func<Task<TResult>> task) where TException : Exception
        {
            return DoWithRetry<TResult, TException>(null, task, false);
        }

        public static Tuple<T, WebException> WithRetryWeb<T>(this Func<T> func)
        {
            return func.WithRetry<T, WebException>();
        }

        public static Task<Tuple<T, WebException>> WithRetryWeb<T>(this Func<Task<T>> task)
        {
            return task.WithRetry<T, WebException>();
        }

        public static Tuple<TResult, Exception> WithRetry<TResult>(this Func<TResult> func)
        {
            return func.WithRetry<TResult, Exception>();
        }

        public static Task<Tuple<TResult, Exception>> WithRetry<TResult>(this Func<Task<TResult>> task)
        {
            return task.WithRetry<TResult, Exception>();
        }

        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity)?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        }

        public static string GetContrahentId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity)?.FindFirst("ContrahentId");

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetEmployeeId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity)?.FindFirst("EmployeeId");

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static StringContent GetJsonContent(this object input)
        {
            string json = JsonConvert.SerializeObject(input);

            return GetJsonContent(json);
        }

        public static StringContent GetJsonContent(this string json) => new(json, Encoding.UTF8, "application/json");

        public static NpgsqlParameter GetNpgsqlParameter(this object value, string name, NpgsqlDbType? type = null)
        {
            name = "@" + name.ToLower();

            NpgsqlParameter result;

            if (type != null)
                result = new NpgsqlParameter(name, type.Value) { Value = value };
            else
                result = new NpgsqlParameter(name, value);

            return result;
        }

        public static string GetErrorString(this Tuple<HttpResponseMessage, Exception> result, bool brief = false)
        {
            string error = null;

            var response = result.Item1;

            if (response == null)
            {
                error = brief ? result.Item2?.Message : result.Item2?.ToString();
            }
            else if (!response.IsSuccessStatusCode)
            {
                error = response.ReasonPhrase + " " + response.Content.ReadAsStringAsync().Result;
            }

            return error;
        }

        public static bool IsSucceeded(this Tuple<HttpResponseMessage, Exception> result) => result.Item1.IsSucceeded();

        public static bool IsSucceeded(this HttpResponseMessage response) => response?.IsSuccessStatusCode ?? false;

        public static int GetMaxId(this IEnumerable<IEntity<int>> entity) => entity.Max(e => e.Id);

        public static int ToInt(this decimal input) => Convert.ToInt32(input.RoundAwayFromZero(0));

        public static decimal RoundAwayFromZero(this decimal input, int decimals) => Math.Round(input, decimals, MidpointRounding.AwayFromZero);

        public static int? GetSize(this object input) => input == null ? null : JsonConvert.SerializeObject(input).GetSize();

        public static T DeepClone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);

            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static TDestination Map<TDestination>(this object source) where TDestination : class, new()
        {
            var destination = new TDestination();

            var destinationPublicProperties = destination.GetProperties().Where(p => p.PropertyType.IsPublic);

            var sourcePublicProperties = source.GetProperties().Where(p => p.PropertyType.IsPublic).ToDictionary(
                p => p.Name,
                p => p
            );

            foreach (var destinationProperty in destinationPublicProperties)
            {
                if (sourcePublicProperties.TryGetValue(destinationProperty.Name, out PropertyInfo sourceProperty))
                {
                    if (destinationProperty.PropertyType.FullName == sourceProperty.PropertyType.FullName)
                    {
                        var sourcePropertyValue = sourceProperty.GetValue(source);

                        destinationProperty.SetValue(destination, sourcePropertyValue);
                    }
                }
            }

            return destination;
        }

        public static TDestination Map<TSource, TDestination>(this TSource source, Action<TSource, TDestination> modifier) where TSource : class where TDestination : class, new()
        {
            var destination = source.Map<TDestination>();

            modifier?.Invoke(source, destination);

            return destination;
        }

        public static T? NullIf<T>(this T left, T right) where T : struct
        {
            return EqualityComparer<T>.Default.Equals(left, right) ? null : left;
        }

        public static T? AsNullable<T>(this T input) where T : struct => (T?)input;

        public static KeyValuePair<TKey, TValue> GetKeyValuePair<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
