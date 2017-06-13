using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class JwtTokenDto
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
