using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace Controllers
{
    [Route("/api/userRoles")]
    public class UserRolesController : Controller
    {
        private readonly IUserRolesRepository _userRolesRepository;

        public UserRolesController(IUserRolesRepository userRolesRepository)
        {
            _userRolesRepository = userRolesRepository;
        }

        // GET api/userRoles
        [HttpGet]        
        public async Task<IEnumerable<string>> Get()
        {
            return (await _userRolesRepository.GetAll()).Select(x => x.Name);
        }

        // GET api/userRoles/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/userRoles/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/userRoles/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
