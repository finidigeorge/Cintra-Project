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
    public class BookingsClient : BaseRestApiClient<BookingDto>, IBookingController
    {
        public BookingsClient() : base(enKnownControllers.BookingsController)
        {
        }


        public async Task<List<BookingDto>> GetAllFiltered(long beginDate, long endDate)
        {
            return await SendRequest<List<BookingDto>>($"api/{ControllerName}/GetAllFiltered/{beginDate}/{endDate}");
        }

        public async Task<CheckResultDto> HasCoachNotOverlappedBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasCoachNotOverlappedBooking", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasCoachScheduleFitBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasCoachScheduleFitBooking", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseNotOverlappedBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasHorseNotOverlappedBooking", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseRequiredBreak(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasHorseRequiredBreak", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseScheduleFitBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasHorseScheduleFitBooking", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseWorkedLessThanAllowed(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/HasHorseWorkedLessThanAllowed", RestSharp.Method.POST, entity);
        }

        public async Task InsertAll(List<BookingDto> entityList)
        {
            await SendRequest<CheckResultDto>($"api/{ControllerName}/InsertAll", RestSharp.Method.POST, entityList);
        }
    }
}
