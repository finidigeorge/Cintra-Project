using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IBaseController<T>
    {
        Task Update(T entity);

        Task<long> Insert(T entity);

        Task Delete(long id);

        Task<List<T>> GetAll();

        Task<T> GetById(long id);
    }
}
