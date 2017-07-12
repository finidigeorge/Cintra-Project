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
    public enum ChangePasswordVmProperties
    {
        ErrorMessage
    }

    public class ChangePasswordVm: BaseVm
    {
        private readonly UsersClient _client = new UsersClient();
        private string _errorMessage;

        public ChangePasswordVm()
        {
            ChangePasswordCommand = new AsyncCommand<object>(async (x) =>
            {                
                await _client.UpdatePassword(Thread.CurrentPrincipal.Identity.Name, x.ToString());
            });
        }

        public bool IsUpdatePasswordSuccess { get; set; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ChangePasswordVmProperties.ErrorMessage)); }
        }

        public IAsyncCommand ChangePasswordCommand { get; set; }
    }
}
