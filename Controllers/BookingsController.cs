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

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class BookingsController : BaseController<Booking, BookingDto>
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
                await _paymentsRepository.SynchronizeWithBooking(booking.Id, ObjectMapper.Map<BookingPayments>(entity.BookingPayment));
                return await _repository.Create(ObjectMapper.Map<Booking>(entity));
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }

        }
    }
}
