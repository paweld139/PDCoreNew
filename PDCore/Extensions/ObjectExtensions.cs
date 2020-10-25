using PDCore.Helpers;
using PDCore.Helpers.Wrappers.DisposableWrapper;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Unity;

namespace PDCore.Extensions
{
    public static class ObjectExtensions
    {
        public static IDisposableWrapper<ISqlRepositoryEntityFramework<TModel>> WrapRepo<TModel>(this ISqlRepositoryEntityFramework<TModel> repo, bool withoutValidation = false) where TModel : class, IModificationHistory
        {
            return new SaveChangesWrapper<TModel>(repo, withoutValidation);
        }

        public static void RemoveRegistrations(this IUnityContainer container, string name, Type registeredType, Type lifetimeManager)
        {
            foreach (var registration in container.Registrations
                .Where(p => p.RegisteredType == (registeredType ?? p.RegisteredType)
                            && p.Name == (name ?? p.Name)
                            && p.LifetimeManager.GetType() == (lifetimeManager ?? p.LifetimeManager.GetType())))
            {
                registration.LifetimeManager.RemoveValue();
            }
        }

        public static void RemoveRegistrations<TReg, TLife>(this IUnityContainer container, string name = null)
        {
            container.RemoveRegistrations(name, typeof(TReg), typeof(TLife));
        }

        public static void RemoveAllRegistrations(this IUnityContainer container)
        {
            container.RemoveRegistrations(null, null, null);
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
    }
}
