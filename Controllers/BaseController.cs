using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Shared.Interfaces;

namespace Controllers
{
    public class BaseController<T, T1> : IBaseController<T1> where T : class where T1 : class
    {
        protected IGenericRepository<T> _repository;

        public BaseController(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        [HttpPost("Update")]
        public virtual async Task Update([FromBody] T1 entity)
        {
           await Task.Run(() => _repository.Update(ObjectMapper.Map<T>(entity)));
        }

        [HttpPost("Insert")]
        public virtual async Task<long> Insert([FromBody] T1 entity)
        {
            return await Task.Run(() => _repository.Insert(ObjectMapper.Map<T>(entity)));
        }

        [HttpDelete]
        public virtual async Task Delete([FromBody]T1 entity)
        {
            await Task.Run(() => _repository.Delete(ObjectMapper.Map<T> (entity)));
        }

        [HttpGet("values")]
        public virtual async Task<List<T1>> GetAll()
        {
            return (await Task.Run(() => _repository.GetAll())).Select(ObjectMapper.Map<T1>).ToList();
        }

        [HttpGet("values/{id}")]
        public virtual async Task<T1> GetById(long id)
        {
            return ObjectMapper.Map<T1>(await Task.Run(() => _repository.GetById(id)));
        }
    }
}
