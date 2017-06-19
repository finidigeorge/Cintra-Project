﻿using System;
using System.Collections.Generic;
using System.Text;
using Shared.Attributes;

namespace Shared.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Boolean NewPasswordOnLogin { get; set; }
    }
}
