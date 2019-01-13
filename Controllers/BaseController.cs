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
        protected readonly ILogger _logger;
        public BaseController(IGenericRepository<T> repository, ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger<BaseController<T, T1>>();
        }                

        [HttpPost]
        public virtual async Task<long> Create([FromBody] T1 entity)
        {
            try
            {
                return await _repository.Create(ObjectMapper.Map<T>(entity));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, entity);
                throw;
            }
            
        }        

        [HttpDelete("{id}")]
        public virtual async Task Delete(long id)
        {
            try
            {
                await _repository.Delete(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, id);
                throw;
            }
            
        }

        [HttpGet]
        public virtual async Task<List<T1>> GetAll()
        {
            try
            {
                return (await _repository.GetAll()).Select(x => ObjectMapper.Map<T1>(x)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, null);
                throw;
            }
            
        }

        [HttpGet("{id}")]
        public virtual async Task<T1> GetById(long id)
        {
            try
            {
                return ObjectMapper.Map<T1>(await _repository.GetById(id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, id);
                throw;
            }
            
        }
    }
}
