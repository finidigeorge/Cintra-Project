using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
     public interface IGenericRepository<T> where T: class
     {
         void Update(T entity);

         Task<long> Insert(T entity);

         void Delete(T entity);

         Task<List<T>> GetAll();

         Task<List<T>> GetByParams(Func<T, bool> where);

         Task<T> GetById(long id);

         Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter);

         Func<T, bool> SimpleComparison(string property, object value);
     }
}
