using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IBaseController<T> where T: class
    {
        Task Update(T entity);

        Task<long> Insert(T entity);

        Task Delete(T entity);

        Task<List<T>> GetAll();

        Task<T> GetById(long id);
    }
}
