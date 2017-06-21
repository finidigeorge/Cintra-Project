using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]")]
    public class UserRolesController : BaseController<UserRoles, UserRoleDto>, IUserRolesController
    {
        public UserRolesController(IUserRolesRepository userRolesRepository) : base(userRolesRepository)
        {
        }

        [HttpGet("getByUser/{login}")]        
        public async Task<IList<UserRoleDto>> GetByUser(string login)
        {            
            return (await ((IUserRolesRepository) _repository).GetByParamsWithUsers(x => x.users.Any(u => u.Login == login))).Select(ObjectMapper.Map<UserRoleDto>).ToList();              
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("values")]
        public override async Task<List<UserRoleDto>> GetAll()
        {
            return (await Task.Run(() => _repository.GetAll())).Select(ObjectMapper.Map<UserRoleDto>).ToList();
        }
    }
}
