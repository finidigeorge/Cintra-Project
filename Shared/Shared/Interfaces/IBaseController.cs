using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IBaseController<T>
    {        
        Task<long> Create(T entity);

        Task Delete(long id);

        Task<List<T>> GetAll();

        Task<T> GetById(long id);
    }
}
