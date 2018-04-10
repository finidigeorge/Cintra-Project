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

        public async Task CancelAllBookings(long metadataId, long FromDate)
        {
            await SendRequest<object>($"api/{ControllerName}/CancelAllBookings/{metadataId}/{FromDate}", RestSharp.Method.POST);
        }

        public async Task<List<BookingDto>> GetAllFiltered(long beginDate, long endDate)
        {
            return await SendRequest<List<BookingDto>>($"api/{ControllerName}/{nameof(IBookingController.GetAllFiltered)}/{beginDate}/{endDate}");
        }

        public async Task<CheckResultDto> HasCoachesNotOverlappedBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasCoachesNotOverlappedBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasCoachesScheduleFitBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasCoachesScheduleFitBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseNotOverlappedBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorseNotOverlappedBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseRequiredBreak(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorseRequiredBreak)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseScheduleFitBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorseScheduleFitBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorseWorkedLessThanAllowed(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorseWorkedLessThanAllowed)}", RestSharp.Method.POST, entity);
        }

        public async Task InsertAll(List<BookingDto> entityList)
        {
            await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.InsertAll)}", RestSharp.Method.POST, entityList);
        }
    }
}
