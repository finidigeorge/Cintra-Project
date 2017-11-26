using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbLayer.Interfaces
{
    public interface IPreservable: IUniqueDto
    {
        bool IsDeleted { get; set; }
    }
}
