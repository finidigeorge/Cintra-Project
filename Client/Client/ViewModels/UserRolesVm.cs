using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Client.Commands;
using RestApi;
using Shared.Dto;

namespace Client.ViewModels
{
    public class UserRolesVm
    {
        private readonly UserRolesClient _userRolesClient = new UserRolesClient();

        public IEnumerable<UserRoleDto> UserRoles { get; private set; }

        public IAsyncCommand GetRolesCommand { get; set; }

        public UserRolesVm()
        {
            GetRolesCommand = new AsyncCommand<object>(async (x) =>
            {
                UserRoles = await _userRolesClient.Get();
            });
        }        
    }
}
