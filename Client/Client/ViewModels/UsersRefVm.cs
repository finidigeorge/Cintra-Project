using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common.DtoMapping;
using Mapping;
using RestApi;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class UsersRefVm : BaseReferenceVm<UserDto, UserDtoUi>
    {
        private readonly UserRolesClient _rolesClient = new UserRolesClient();
        public List<UserRoleDtoUi> UserRoles { get; set; }

        public UsersRefVm()
        {
            Client = new UsersClient();

            RefreshDataCommand = new AsyncCommand<object>(async (x) =>
            {
                await GetItemsCommand.ExecuteAsync(x);
                UserRoles = (await _rolesClient.GetAll()).Select(r => ObjectMapper.Map<UserRoleDtoUi>(r)).ToList();
            });
        }

       
    }

}
