using System;
using System.Collections.Generic;
using Shared;
using Shared.Dto;
using Shared.Dto.Interfaces;
using Shared.Interfaces;

namespace RestClient
{
    public static class RestClientFactory
    {
        private static readonly Dictionary<Type, string> _controllers = new Dictionary<Type, string>()
        {
            { typeof(CoachDto), enKnownControllers.CoachesController },
            { typeof(HorseDto), enKnownControllers.HorsesController },
            { typeof(ScheduleDto), enKnownControllers.SchedulesController },
            { typeof(ScheduleDataDto), enKnownControllers.SchedulesDataController},
            { typeof(ServiceDto), enKnownControllers.ServicesController},            

        };

        public static IBaseController<T> GetClient<T>() where T: class, IUniqueDto
        {
            return new BaseRestApiClient<T>(_controllers[typeof(T)]);
        }
    }
}
