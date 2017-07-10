using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Shared.Dto;

namespace RestClient
{
    public class UsersClient: BaseRestApiClient<UserDto>
    {
        public UsersClient() : base("users")
        {            
        }

        public async void UpdatePassword(long userId, string password)
        {
            await SendRequest<UserDto>($"api/{ControllerName}/updatePassword", Method.PUT, new UserDto() {Id = userId, Password = password});
        }

        public async void ResetPassword(long userId)
        {
            await SendRequest<UserDto>($"api/{ControllerName}/resetPassword", Method.PUT, new UserDto() { Id = userId });
        }
    }
}
