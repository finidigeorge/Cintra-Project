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

        public async Task<CheckResultDto> HasHorsesNotOverlappedBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorsesNotOverlappedBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorsesRequiredBreak(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorsesRequiredBreak)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorsesScheduleFitBooking(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorsesScheduleFitBooking)}", RestSharp.Method.POST, entity);
        }

        public async Task<CheckResultDto> HasHorsesWorkedLessThanAllowed(BookingDto entity)
        {
            return await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.HasHorsesWorkedLessThanAllowed)}", RestSharp.Method.POST, entity);
        }

        public async Task InsertAll(List<BookingDto> entityList)
        {
            await SendRequest<CheckResultDto>($"api/{ControllerName}/{nameof(IBookingController.InsertAll)}", RestSharp.Method.POST, entityList);
        }
    }
}
