using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB.Data;

namespace Repositories.Interfaces
{
     public interface IGenericRepository<T>
     {
         Task<long> Create(T entity, DataConnection dbContext = null);

         Task Delete(long id, DataConnection dbContext = null);

         Task<List<T>> GetAll(DataConnection dbContext = null);

         Task<List<T>> GetByParams(Expression<Func<T, bool>> where, DataConnection dbContext = null);

         Task<T> GetById(long id, DataConnection dbContext = null);

         Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter, DataConnection dbContext = null);

        Expression<Func<T, bool>> SimpleComparison(string property, object value);
     }
}
