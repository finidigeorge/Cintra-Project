using LinqToDB;
using LinqToDB.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DbLayer.Extentions
{
    public static class Linq2dbSqliteExtentions
    {
        private static readonly ReaderWriterLockSlim _semaphore = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        //because sqlite doesn't allow to run concurrent write dml's
        public static async Task<dynamic> ApplyWriteLock(Func<Task<dynamic>> fetcher)
        {
            _semaphore.EnterWriteLock();
            try
            {
                return await fetcher();
            }
            finally
            {
                _semaphore.ExitWriteLock();
            }
        }

        public static async Task<dynamic> ApplyReadLock(Func<Task<dynamic>> fetcher)
        {
            _semaphore.EnterUpgradeableReadLock();
            try
            {
                return await fetcher();
            }
            finally
            {
                _semaphore.ExitUpgradeableReadLock();
            }
        }

        public static async Task<int> DeleteAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await dataContext.DeleteAsync(obj, token));
        }

        public static async Task<int> DeleteAsyncWithLock<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await source.DeleteAsync(predicate, token));
        }

        public static async Task<int> DeleteAsyncWithLock<T>(this IQueryable<T> source, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await source.DeleteAsync(token));
        }

        public static async Task<object> InsertWithIdentityAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await dataContext.InsertWithIdentityAsync(obj, token));
        }

        public static async Task<int> UpdateAsyncWithLock<T>(this IUpdatable<T> source, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await source.UpdateAsync(token));
        }

        public static async Task<int> UpdateAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await ApplyWriteLock(async () => await dataContext.UpdateAsync(obj, token));
        }
    }
}
