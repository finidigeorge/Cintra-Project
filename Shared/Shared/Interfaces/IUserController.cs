using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;

namespace Shared.Interfaces
{
    public interface IUserController
    {
        Task UpdatePassword(UserDto entity);
        Task ResetPassword(UserDto entity);
    }
}
