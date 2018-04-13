using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IHorseController
    {
        Task<List<HorseDto>> GetAllByService(long serviceId);
    }
}
