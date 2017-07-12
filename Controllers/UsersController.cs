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
        private readonly IAuthController _authController;
        private readonly IUsersRepository repository;

        public UsersController(IHttpContextAccessor httpContextAccessor, IUsersRepository repository, IAuthController authController) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _authController = authController;
            repository = (IUsersRepository) _repository;
        }

        [HttpPut]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task Update(UserDto entity)
        {            
            entity.Password = (await base.GetById(entity.Id)).Password;
            await base.Update(entity);
        }

        [HttpPut("/api/[controller]/updatePassword")]        
        public async Task UpdatePassword(UserDto entity)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var _entity = await repository.GetByLogin(entity.Login);

            if (!await repository.HasAdminAccess(userName))
                if (_entity.Login != userName)
                    throw new Exception("You need to have the Administrator role to complete this operation");

            
            _entity.Password = _authController.GetPassword(entity.Password);

            await repository.Update(_entity);
        }

        [HttpPut("/api/[controller]/resetPassword")]
        [Authorize(Roles = enUserRoles.Administrator)]
        public async Task ResetPassword([FromBody]UserDto entity)
        {            
            var _entity = await repository.GetByLogin(entity.Login);
            _entity.Password = _authController.GetPassword(_entity.Login);

            await repository.Update(_entity);
        }

        [HttpPost]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task<long> Insert([FromBody]UserDto entity)
        {
            return await base.Insert(entity);
        }

        [HttpDelete]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override async Task Delete([FromBody]UserDto entity)
        {
            await base.Delete(entity);
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
