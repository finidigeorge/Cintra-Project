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
        public BookingsController(IGenericRepository<Booking> repository, ILoggerFactory loggerFactory) : base(repository, loggerFactory)
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
    }
}
