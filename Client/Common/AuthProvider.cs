using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class AuthProvider
    {
        private static object _lock = new object();
        private static JwtTokenDto _authToken;

        public static void SetToken(JwtTokenDto token)
        {
            lock (_lock)
            {
                _authToken = token;
            }
        }

        public static JwtTokenDto GetToken()
        {
            return _authToken;
        }

    }
}
