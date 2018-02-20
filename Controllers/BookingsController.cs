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
using Shared.Extentions;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class BookingsController : BaseController<Booking, BookingDto>, IBookingController
    {
        private readonly IBookingPaymentsRepository _paymentsRepository;
        private readonly IBookingTemplatesMetadataRepository _bookingsMetadataRepository;
        private readonly IGenericRepository<BookingTemplates> _templatesRepository = new GenericRepository<BookingTemplates>();


        private IBookingRepository repository { get => (IBookingRepository)_repository; }

        public BookingsController(IBookingRepository repository, 
                IBookingPaymentsRepository paymentsRepository,
                IBookingTemplatesMetadataRepository bookingsMetadataRepository,                
                ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
            _paymentsRepository = paymentsRepository;
            _bookingsMetadataRepository = bookingsMetadataRepository;            
        }

        [HttpPost]
        public override async Task<long> Create([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                if (booking.BookingsTemplateMetadata != null)
                {
                    var metadataId = await _bookingsMetadataRepository.Create(booking.BookingsTemplateMetadata);
                    foreach (var p in booking.BookingsTemplateMetadata.BookingTemplates) {
                        p.TemplateMetadataId = metadataId;
                        await _templatesRepository.Create(p);
                    }

                    booking.TemplateMetadataId = metadataId;
                }

                var bookingId = await _repository.Create(booking);
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

        [HttpPost("/api/[controller]/CancelAllBookings/{metadataId}/{FromDate}")]
        public async Task CancelAllBookings(long metadataId, long FromDate)
        {
            try
            {
                var _FromDate = DateTime.FromBinary(FromDate);
                await _bookingsMetadataRepository.CancelAllBookings(metadataId, _FromDate);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message);
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

                await ((BookingTemplatesMetadataRepository)_bookingsMetadataRepository).RunWithinTransaction(async (db) =>
                {
                    
                    var tmpDate = _beginDate.TruncateToDayStart();
                    while (tmpDate < _endDate)
                    {
                        await _bookingsMetadataRepository.GenerateAllPermanentBookings(tmpDate, db);
                        tmpDate = tmpDate.AddDays(1);
                    }

                    return null;
                });
                              
                var res = new List<BookingDto>();

                foreach (var b in (await _repository.GetByParams(x => !(x.BeginTime > _endDate || x.EndTime < _beginDate)))) {
                    var item = ObjectMapper.Map<BookingDto>(b);
                    item.ValidationErrors = await repository.RunValidations(b, true);
                    item.ValidationWarnings = await repository.RunValidations(b, false);
                    res.Add(item);
                }

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
