using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Shared.Dto;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]    
    public class UserRolesController : Controller, IUserRolesController
    {
        private readonly IUserRolesRepository _userRolesRepository;

        public UserRolesController(IUserRolesRepository userRolesRepository)
        {
            _userRolesRepository = userRolesRepository;
        }

        // GET api/userRoles
        [HttpGet]
        [Route("/api/userRoles/{login}")]
        public async Task<IList<UserRoleDto>> Get(string login)
        {
            return (await _userRolesRepository.GetByParamsWithUsers(x => true)).Select(ObjectMapper.Map<UserRoleDto>).ToList();                        
        }        
    }
}
