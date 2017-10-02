using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Commands;
using RestClient;
using Shared.Dto;

namespace Client.ViewModels
{
    public class ChangePasswordVm: BaseVm
    {
        private readonly UsersClient _client = new UsersClient();       

        public ChangePasswordVm()
        {
            ChangePasswordCommand = new AsyncCommand<object>(async (x) =>
            {
                try
                {
                    await _client.UpdatePassword(Thread.CurrentPrincipal.Identity.Name, x.ToString());
                    IsUpdatePasswordSuccess = true;
                }
                catch (Exception e)
                {
                    IsUpdatePasswordSuccess = false;
                    ErrorMessage = e.Message;
                }

            }, x => true);
        }

        public bool IsUpdatePasswordSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public IAsyncCommand ChangePasswordCommand { get; set; }
    }
}
