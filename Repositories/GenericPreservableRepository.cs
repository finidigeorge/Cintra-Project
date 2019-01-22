using DataModels;
using DbLayer.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class GenericPreservableRepository<T> : GenericRepository<T> where T : class, IPreservable
    {
        public override async Task Delete(long id, DataConnection dbContext = null)
        {
            await RunWithinTransaction(async (db) =>
            {
                var pkName = typeof(T).GetProperties().First(prop => prop.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                var expression = SimpleComparison(pkName.Name, id);

                var entity = db.GetTable<T>().Where(expression).FirstOrDefault();
                entity.IsDeleted = true;
                await db.UpdateAsync(entity);
                
                return null;
            }, dbContext);
        }

        public override async Task<List<T>> GetAll(DataConnection dbContext = null)
        {
            return (await GetByParams((x) => x.IsDeleted == false, dbContext)).ToList();
        }

        public override async Task<List<T>> GetByParams(Expression<Func<T, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await db.GetTable<T>().Where(where).Where((x) => x.IsDeleted == false).ToListAsync();
            }, dbContext);
        }

        public override async Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter, DataConnection dbContext = null)

        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new Exception("Property name is empty");

            var expression = SimpleComparison(propertyName, valueToFilter);

            return await RunWithinTransaction(async (db) =>
            {
                return await db.GetTable<T>().Where(expression).Where((x) => x.IsDeleted == false).ToListAsync();
            }, dbContext);
        }
    }
}
