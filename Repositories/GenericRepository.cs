using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using LinqToDB.Mapping;
using Repositories.Interfaces;

namespace Repositories
{
    public class GenericRepository<T>: IGenericRepository<T> where T : class
    {
        public async Task Update(T entity)
        {
            using (var db = new CintraDB())
            {
                await Task.FromResult(db.Update(entity));
            }
        }

        public async Task<long> Insert(T entity)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult((long)db.InsertWithIdentity(entity));
            }
        }

        public async Task Delete(long id)
        {
            using (var db = new CintraDB())
            {
                var pkName = typeof(T).GetProperties().First(prop => prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                var expression = SimpleComparison(pkName.Name, id);

                await Task.FromResult(db.Delete(db.GetTable<T>().Where(expression).FirstOrDefault()));
            }
        }

        public async Task<List<T>> GetAll()
        {
            using (var db = new CintraDB())
            {
                return await db.GetTable<T>().ToListAsync();
            }
        }

        public async Task<List<T>> GetByParams(Func<T, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(db.GetTable<T>().Where(where).ToList());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">linqToDb Table mapped</typeparam>
        /// <param name="id"> Have to be of the same type of primary key atribute of T table mapped</param>
        /// <returns>T linqToDb mapped class</returns>
        public async Task<T> GetById(long id)
        {
            using (var db = new CintraDB())
            {
                var pkName = typeof(T).GetProperties().First(prop => prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                var expression = SimpleComparison(pkName.Name, id);

                return await Task.FromResult(db.GetTable<T>().Where(expression).FirstOrDefault());
            }
        }

        /// <summary>
        /// Excelent to use to get entities by FK
        /// </summary>
        /// <typeparam name="T">Entity To Filter From DB Mapped</typeparam>
        /// <typeparam name="D">Type of property to filter using Equals Comparer</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <param name="valueToFilter">Value to filter query</param>
        /// <returns>List of T</returns>
        public async Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter)
           
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new Exception("Property name is empty");

            var expression = SimpleComparison(propertyName, valueToFilter);

            using (var db = new CintraDB())
            {
                return await Task.FromResult(db.GetTable<T>().Where(expression).ToList());                
            }
        }

        public  Func<T, bool> SimpleComparison(string property, object value)
        {
            var type = typeof(T);
            var pe = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(pe, property);
            var constantReference = Expression.Constant(value);

            return Expression.Lambda<Func<T, bool>>
            (Expression.Equal(propertyReference, constantReference), pe).Compile();
        }

        private  Func<T, bool> SimpleComparison<D>(string propertyName, D value)
        {
            var type = typeof(T);
            var pe = Expression.Parameter(type, "p");
            var constantReference = Expression.Constant(value);
            var propertyReference = Expression.Property(pe, propertyName);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Equal(propertyReference, constantReference), pe).Compile();
        }
    }
}
