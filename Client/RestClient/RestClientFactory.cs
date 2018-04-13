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
        private static readonly Dictionary<Type, string> _baseControllers = new Dictionary<Type, string>()
        {
            { typeof(BookingDto), enKnownControllers.BookingsController },            
            { typeof(ClientDto), enKnownControllers.ClientsController},            
            { typeof(PaymentTypeDto), enKnownControllers.PaymentTypesController },
            { typeof(ScheduleDto), enKnownControllers.SchedulesController },            
            { typeof(ServiceDto), enKnownControllers.ServicesController},            

        };

        public static IBaseController<T> GetClient<T>() where T: class, IUniqueDto
        {
            if (_baseControllers.ContainsKey(typeof(T)))
                return new BaseRestApiClient<T>(_baseControllers[typeof(T)]);

            else
                throw new Exception($"Unknown object type {typeof(T).Name}");
        }
    }
}
