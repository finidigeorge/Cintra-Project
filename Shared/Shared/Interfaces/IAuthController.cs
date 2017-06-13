using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;

namespace Shared.Interfaces
{
    public interface IAuthController
    {
        Task<JwtTokenDto> Login(UserDto applicationUser);
        string GetPassword(string password);
    }
}
