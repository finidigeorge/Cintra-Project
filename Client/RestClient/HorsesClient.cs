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
    public class HorsesClient: BaseRestApiClient<HorseDto>, IHorseController
    {
        public HorsesClient() : base(enKnownControllers.HorsesController) { }

        public async Task<List<HorseDto>> GetAllByService(long serviceId)
        {
            return await SendRequest<List<HorseDto>>($"api/{ControllerName}/{nameof(IHorseController.GetAllByService)}/{serviceId}");
        }
    }
}
