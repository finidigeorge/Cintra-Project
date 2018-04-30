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
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        //because sqlite doesn't allow to run concurrent write dml's
        private static async Task<dynamic> _runWithLock(Func<Task<dynamic>> fetcher)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await fetcher();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public static async Task<int> DeleteAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await dataContext.DeleteAsync(obj, token));
        }

        public static async Task<int> DeleteAsyncWithLock<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await source.DeleteAsync(predicate, token));
        }

        public static async Task<int> DeleteAsyncWithLock<T>(this IQueryable<T> source, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await source.DeleteAsync(token));
        }

        public static async Task<object> InsertWithIdentityAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await dataContext.InsertWithIdentityAsync(obj, token));
        }

        public static async Task<int> UpdateAsyncWithLock<T>(this IUpdatable<T> source, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await source.UpdateAsync(token));
        }

        public static async Task<int> UpdateAsyncWithLock<T>(this IDataContext dataContext, T obj, CancellationToken token = default(CancellationToken))
        {
            return await _runWithLock(async () => await dataContext.UpdateAsync(obj, token));
        }
    }
}
