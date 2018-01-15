using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Repositories.Interfaces;
using System.Threading.Tasks;
using Mapping;
using Repositories;
using System.Linq;
using Shared.Interfaces;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class BookingsController : BaseController<Booking, BookingDto>, IBookingController
    {
        private readonly BookingPaymentsRepository _paymentsRepository = new BookingPaymentsRepository();
        private IBookingRepository repository { get => (IBookingRepository)_repository; }


        public BookingsController(IBookingRepository repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {            
        }

        [HttpPost]
        public override async Task<long> Create([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var bookingId = await _repository.Create(ObjectMapper.Map<Booking>(entity));
                await _paymentsRepository.SynchronizeWithBooking(bookingId, ObjectMapper.Map<BookingPayments>(entity.BookingPayment));
                return bookingId;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/InsertAll")]
        public async Task InsertAll([FromBody] List<BookingDto> entityList)
        {
            try
            {
                await ((BookingRepository)_repository).RunWithinTransaction(async (db) =>
                {
                    foreach (var e in entityList)
                        await Create(e);

                    return null;
                });
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entityList);
                throw;
            }
        }

        [HttpGet("/api/[controller]/GetAllFiltered/{beginDate}/{endDate}")]
        public virtual async Task<List<BookingDto>> GetAllFiltered(long beginDate, long endDate)
        {
            try
            {
                var _beginDate = DateTime.FromBinary(beginDate);
                var _endDate = DateTime.FromBinary(endDate);
               
                var res = (await _repository.GetByParams(x => !(x.BeginTime > _endDate || x.EndTime < _beginDate))).Select(x => ObjectMapper.Map<BookingDto>(x)).ToList();
                return res;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, null);
                throw;
            }

        }

        [HttpPost("/api/[controller]/HasCoachNotOverlappedBooking")]
        public async Task<CheckResultDto> HasCoachNotOverlappedBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var coach = booking.Coach;

                return await repository.HasCoachNotOverlappedBooking(coach, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorseNotOverlappedBooking")]
        public async Task<CheckResultDto> HasHorseNotOverlappedBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var horse = booking.Hor;

                return await repository.HasHorseNotOverlappedBooking(horse, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorseRequiredBreak")]
        public async Task<CheckResultDto> HasHorseRequiredBreak([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var horse = booking.Hor;

                return await repository.HasHorseRequiredBreak(horse, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorseWorkedLessThanAllowed")]
        public async Task<CheckResultDto> HasHorseWorkedLessThanAllowed([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var horse = booking.Hor;

                return await repository.HasHorseWorkedLessThanAllowed(horse, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorseScheduleFitBooking")]
        public async Task<CheckResultDto> HasHorseScheduleFitBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var horse = booking.Hor;

                return await repository.HasHorseScheduleFitBooking(horse, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasCoachScheduleFitBooking")]
        public async Task<CheckResultDto> HasCoachScheduleFitBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var coach = booking.Coach;

                return await repository.HasCoachScheduleFitBooking(coach, booking);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }
    }
}
