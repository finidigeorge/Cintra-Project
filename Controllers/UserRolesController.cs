using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModels;
using Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using Shared;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class UserRolesController : BaseController<UserRoles, UserRoleDto>, IUserRolesController
    {
        public UserRolesController(IUserRolesRepository userRolesRepository, ILoggerFactory loggerFactory) : base(userRolesRepository, loggerFactory)
        {
        }

        [HttpGet("/api/[controller]/getByUser/{login}")]        
        public async Task<IList<UserRoleDto>> GetByUser(string login)
        {            
            return (await ((IUserRolesRepository) _repository).GetByParamsWithUsers(x => x.users.Any(u => u.Login == login))).Select(ObjectMapper.Map<UserRoleDto>).ToList();              
        }

        [Authorize(Roles = nameof(UserRolesEnum.Administrator))]
        [HttpGet]
        public override async Task<List<UserRoleDto>> GetAll()
        {
            return (await Task.Run(() => _repository.GetAll())).Select(ObjectMapper.Map<UserRoleDto>).ToList();
        }
    }
}
