using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using LinqToDB.Mapping;
using Repositories.Interfaces;
using Shared.Dto.Interfaces;
using DbLayer.Interfaces;
using System.Threading;
using DbLayer.Extentions;

namespace Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IUniqueDto
    {
        public async Task<dynamic> RunWithinTransaction(Func<CintraDB, Task<dynamic>> fetcher, CintraDB dbContext = null)
        {
            var isTransactional = dbContext == null;
            bool isSuccess = true;
            var db = dbContext ?? new CintraDB();

            try
            {
                if (isTransactional)
                    db.BeginTransaction();

                return await Linq2dbSqliteExtentions.ApplyReadLock(async () => await fetcher(db));
            }
            catch (Exception)
            {
                isSuccess = false;
                if (isTransactional)
                {
                    db.RollbackTransaction();
                }
                throw;
            }
            finally
            {
                if (isTransactional && isSuccess)
                    db.CommitTransaction();

                if (isTransactional)
                    db.Dispose();
            }
        }

        public virtual async Task<long> Create(T entity, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                if (entity.Id == 0)
                {
                    entity.Id = (long)await db.InsertWithIdentityAsyncWithLock(entity);
                }
                else
                    await db.UpdateAsyncWithLock(entity);

                return entity.Id;
            }, dbContext);
        }

        public virtual async Task Delete(long id, CintraDB dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                var pkName = typeof(T).GetProperties().First(prop => prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                var expression = SimpleComparison(pkName.Name, id);

                await db.DeleteAsyncWithLock(db.GetTable<T>().Where(expression).FirstOrDefault());

                return null;
            }, dbContext);
        }

        public virtual async Task<List<T>> GetAll(CintraDB dbContext = null)
        {
            return (await GetByParams((x) => true, dbContext)).ToList();
        }

        public virtual async Task<List<T>> GetByParams(Expression<Func<T, bool>> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(db.GetTable<T>().Where(where).ToList());
            }, dbContext);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">linqToDb Table mapped</typeparam>
        /// <param name="id"> Have to be of the same type of primary key atribute of T table mapped</param>
        /// <returns>T linqToDb mapped class</returns>
        public virtual async Task<T> GetById(long id, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                var pkName = typeof(T).GetProperties().First(prop => prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                var expression = SimpleComparison(pkName.Name, id);

                return (await GetByParams(expression, db)).FirstOrDefault();
            }, dbContext);
        }

        /// <summary>
        /// Excelent to use to get entities by FK
        /// </summary>
        /// <typeparam name="T">Entity To Filter From DB Mapped</typeparam>
        /// <typeparam name="D">Type of property to filter using Equals Comparer</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <param name="valueToFilter">Value to filter query</param>
        /// <returns>List of T</returns>
        public virtual async Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter, CintraDB dbContext = null)

        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new Exception("Property name is empty");

            var expression = SimpleComparison(propertyName, valueToFilter);

            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(db.GetTable<T>().Where(expression).ToList());
            }, dbContext);
        }

        public Expression<Func<T, bool>> SimpleComparison(string property, object value)
        {
            var type = typeof(T);
            var pe = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(pe, property);
            var constantReference = Expression.Constant(value);

            return Expression.Lambda<Func<T, bool>>
            (Expression.Equal(propertyReference, constantReference), pe);
        }

        private Expression<Func<T, bool>> SimpleComparison<D>(string propertyName, D value)
        {
            var type = typeof(T);
            var pe = Expression.Parameter(type, "p");
            var constantReference = Expression.Constant(value);
            var propertyReference = Expression.Property(pe, propertyName);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Equal(propertyReference, constantReference), pe);
        }
    }
}
