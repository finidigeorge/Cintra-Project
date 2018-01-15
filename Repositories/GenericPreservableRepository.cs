using DataModels;
using DbLayer.Interfaces;
using LinqToDB;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class GenericPreservableRepository<T> : GenericRepository<T> where T : class, IPreservable
    {
        public override async Task Delete(long id, CintraDB dbContext = null)
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

        public override async Task<List<T>> GetAll()
        {
            return (await GetByParams((x) => x.IsDeleted == false)).ToList();
        }

        public override async Task<List<T>> GetByParams(Func<T, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(db.GetTable<T>().Where(where).Where((x) => x.IsDeleted == false).ToList());
            }, dbContext);
        }

        public override async Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter)

        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new Exception("Property name is empty");

            var expression = SimpleComparison(propertyName, valueToFilter);

            using (var db = new CintraDB())
            {
                return await Task.FromResult(db.GetTable<T>().Where(expression).Where((x) => x.IsDeleted == false).ToList());
            }
        }
    }
}
