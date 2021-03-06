﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common.DtoMapping;
using RestApi;
using Shared.Dto;

namespace Client.ViewModels
{
    public class UserRolesRefVm : BaseReferenceVm<UserRoleDto, UserRoleDtoUi>
    {
        public UserRolesRefVm()
        {
            Client = new UserRolesClient();
        }
        
    }
}
