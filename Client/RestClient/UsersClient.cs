using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Shared;
using Shared.Dto;

namespace RestClient
{
    public class UsersClient: BaseRestApiClient<UserDto>
    {
        public UsersClient() : base(enKnownControllers.UsersController)
        {            
        }

        public async Task UpdatePassword(string login, string password)
        {
            await SendRequest<UserDto>($"api/{ControllerName}/updatePassword", Method.PUT, new UserDto() {Login = login, Password = password});
        }

        public async Task ResetPassword(string login)
        {
            await SendRequest<UserDto>($"api/{ControllerName}/resetPassword", Method.PUT, new UserDto() { Login = login });
        }
    }
}
