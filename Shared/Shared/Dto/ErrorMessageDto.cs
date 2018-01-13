using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dto
{
    public class ErrorMessageDto
    {
        public string ErrorMessage { get; set; }
    }

    public class CheckResultDto : ErrorMessageDto
    {
        public bool Result { get; set; }
    }
}
