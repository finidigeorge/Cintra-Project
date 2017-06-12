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
        [Route("/api/userRoles")]
        public async Task<IEnumerable<UserRoleDto>> Get()
        {
            return (await _userRolesRepository.GetAll()).Select(ObjectMapper.Map<UserRoleDto>);                        
        }        
    }
}
