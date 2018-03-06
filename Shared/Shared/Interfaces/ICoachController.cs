using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface ICoachController
    {
        Task<List<CoachDto>> GetAllByService(long serviceId, bool onlyAssignedCoaches);
    }
}
