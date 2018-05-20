using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace Repositories.Interfaces
{
     public interface IGenericRepository<T>
     {
         Task<long> Create(T entity, CintraDB dbContext = null);

         Task Delete(long id, CintraDB dbContext = null);

         Task<List<T>> GetAll();

         Task<List<T>> GetByParams(Expression<Func<T, bool>> where, CintraDB dbContext = null);

         Task<T> GetById(long id);

         Task<List<T>> GetByPropertyValue<D>(string propertyName, D valueToFilter);

        Expression<Func<T, bool>> SimpleComparison(string property, object value);
     }
}
