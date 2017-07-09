using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestApi;
using RestSharp;
using Shared;
using Shared.Dto;
using Shared.Interfaces;

namespace RestClient
{
    public class AuthClient : BaseRestApiClient<UserDto>, IAuthController
    {
        public string GetPassword(string password)
        {
            throw new NotImplementedException();
        }

        public async Task<JwtTokenDto> Login(UserDto applicationUser)
        {
            return await SendRequest<JwtTokenDto>("/api/auth/login", Method.POST, body: applicationUser);
        }

        public AuthClient() : base(enKnownControllers.AuthController)
        {
        }
    }
}
