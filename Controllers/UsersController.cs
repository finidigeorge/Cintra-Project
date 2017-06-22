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
        public UsersController(IUsersRepository repository) : base(repository)
        {
        }

        [HttpPut]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override Task Update(UserDto entity)
        {
            return base.Update(entity);
        }

        [HttpPost]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override Task<long> Insert(UserDto entity)
        {
            return base.Insert(entity);
        }

        [HttpDelete]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override Task Delete(UserDto entity)
        {
            return base.Delete(entity);
        }


        [HttpGet]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override Task<List<UserDto>> GetAll()
        {
            return base.GetAll();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = enUserRoles.Administrator)]
        public override Task<UserDto> GetById(long id)
        {
            return base.GetById(id);
        }
        
    }
}
