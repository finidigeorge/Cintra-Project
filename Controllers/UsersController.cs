using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repositories.Interfaces;
using Shared;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class UsersController: BaseController<User, UserDto>, IUserController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthRepository _authRepository;
        private readonly IUsersRepository repository;
        private readonly ILogger _logger;

        public UsersController(IHttpContextAccessor httpContextAccessor, IUsersRepository repository, IAuthRepository authRepository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authRepository = authRepository;
            this.repository = (IUsersRepository) _repository;
            _logger = loggerFactory.CreateLogger<UsersController>();
        }

        [HttpPut]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task Update([FromBody]UserDto entity)
        {            
            entity.Password = (await base.GetById(entity.Id)).Password;
            await base.Update(entity);
        }

        [HttpPut("/api/[controller]/updatePassword")]        
        public async Task UpdatePassword([FromBody]UserDto entity)
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var _entity = await repository.GetByLogin(entity.Login);

                if (!await repository.HasAdminAccess(userName))
                    if (_entity.Login != userName)
                        throw new Exception("You need to have the Administrator role to complete this operation");


                _entity.Password = _authRepository.GeneratePassword(entity.Password);
                _entity.NewPasswordOnLogin = false;

                await repository.Update(_entity);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }           
        }

        [HttpPut("/api/[controller]/resetPassword")]
        [Authorize(Roles = enUserRoles.Administrator)]
        public async Task ResetPassword([FromBody]UserDto entity)
        {
            try
            {
                var _entity = await repository.GetByLogin(entity.Login);
                _entity.NewPasswordOnLogin = true;
                _entity.Password = _authRepository.GeneratePassword(_entity.Login);

                await repository.Update(_entity);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }            
        }

        [HttpPost]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task<long> Insert([FromBody]UserDto entity)
        {
            return await base.Insert(entity);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task Delete(long id)
        {
            await base.Delete(id);
        }


        [HttpGet]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task<List<UserDto>> GetAll()
        {
            var res = await base.GetAll();
            res.ForEach(x => x.Password = null);
            return res;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task<UserDto> GetById(long id)
        {
            var res = await base.GetById(id);
            res.Password = null;
            return res;
        }
        
    }
}
