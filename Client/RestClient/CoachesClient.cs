using Shared;
using Shared.Dto;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    public class CoachesClient : BaseRestApiClient<CoachDto>, ICoachController
    {
        public CoachesClient() : base(enKnownControllers.CoachesController) { }

        public async Task<List<CoachDto>> GetAllByService(long serviceId, bool onlyAssignedCoaches)
        {
            return await SendRequest<List<CoachDto>>($"api/{ControllerName}/{nameof(ICoachController.GetAllByService)}/{serviceId}/{onlyAssignedCoaches}");
        }
    }
}
