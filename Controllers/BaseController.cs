using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Shared.Interfaces;

namespace Controllers
{
    public class BaseController<T, T1> : IBaseController<T1>
    {
        protected IGenericRepository<T> _repository;
        private readonly ILogger _logger;
        public BaseController(IGenericRepository<T> repository, ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger<BaseController<T, T1>>();
        }        

        [HttpPut]
        public virtual async Task Update([FromBody] T1 entity)
        {
            try
            {
                await Task.Run(() => _repository.Update(ObjectMapper.Map<T>(entity)));
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
           
        }

        [HttpPost]
        public virtual async Task<long> Insert([FromBody] T1 entity)
        {
            try
            {
                return await Task.Run(() => _repository.Insert(ObjectMapper.Map<T>(entity)));
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
            
        }

        [HttpDelete]
        public virtual async Task Delete([FromBody]T1 entity)
        {
            try
            {
                await Task.Run(() => _repository.Delete(ObjectMapper.Map<T>(entity)));
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
            
        }

        [HttpGet]
        public virtual async Task<List<T1>> GetAll()
        {
            try
            {
                return (await Task.Run(() => _repository.GetAll())).Select(x => ObjectMapper.Map<T1>(x)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, null);
                throw;
            }
            
        }

        [HttpGet("{id}")]
        public virtual async Task<T1> GetById(long id)
        {
            try
            {
                return ObjectMapper.Map<T1>(await Task.Run(() => _repository.GetById(id)));
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, id);
                throw;
            }
            
        }
    }
}
