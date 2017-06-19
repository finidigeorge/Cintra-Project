using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Shared.Dto;

namespace Client.ViewModels
{
    public class UsersRefVm : BaseReferenceVm<UserDto>
    {
        //private readonly UsersClient _usersClient = new UsersClient();

        public UsersRefVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                //Items = await _usersClient.Get(Thread.CurrentPrincipal.Identity.Name);
            });

            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                GetItemsCommand.ExecuteAsync(null);
        }
    }

}
