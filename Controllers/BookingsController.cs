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
using Shared;

namespace Controllers
{
    [Authorize]
    [Route("/api/[controller]/values")]
    public class BookingsController : BaseController<Booking, BookingDto>, IBookingController
    {
        private readonly IBookingPaymentsRepository _paymentsRepository;
        private readonly IBookingTemplatesMetadataRepository _bookingsMetadataRepository;
        private readonly IBookingTemplateRepository _templatesRepository;


        private IBookingRepository repository { get => (IBookingRepository)_repository; }

        public BookingsController(IBookingRepository repository, 
                IBookingPaymentsRepository paymentsRepository,
                IBookingTemplatesMetadataRepository bookingsMetadataRepository,
                IBookingTemplateRepository templatesRepository,
                ILoggerFactory loggerFactory) : base(repository, loggerFactory)
        {
            _paymentsRepository = paymentsRepository;
            _bookingsMetadataRepository = bookingsMetadataRepository;
            _templatesRepository = templatesRepository;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRolesEnum.Administrator))]
        public override async Task<long> Create([FromBody] BookingDto entity)
        {
            if (entity.Id == 0)
                return await CreateInternal(entity);
            else
                throw new Exception("Can't update an existing booking using Create call");
        }

        [HttpPatch]
        [Authorize(Roles = nameof(UserRolesEnum.Administrator))]
        public async Task<long> Edit([FromBody] BookingDto entity)
        {
            return await CreateInternal(entity);
        }

        private async Task<long> CreateInternal(BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);

                await ((BookingRepository)_repository).RunWithinTransaction(async (db) =>
                {
                    //write booking template to database only if we create it
                    //updates are not supported currently
                    if (booking.BookingsTemplateMetadata != null && booking.BookingsTemplateMetadata.Id == 0)
                    {
                        var metadata = booking.BookingsTemplateMetadata;
                        await _bookingsMetadataRepository.Create(metadata, db);
                        foreach (var p in booking.BookingsTemplateMetadata.BookingTemplates)
                        {
                            p.TemplateMetadataId = metadata.Id;
                            p.BookingsTemplateMetadata = booking.BookingsTemplateMetadata;
                            await _templatesRepository.Create(p, db);
                        }

                        booking.TemplateMetadataId = metadata.Id;
                    }

                    return null;
                });

                return await ((BookingRepository)_repository).RunWithinTransaction(async (db) => 
                { 
                    var bookingId = await _repository.Create(booking);
                    await _paymentsRepository.SynchronizeWithBooking(bookingId, ObjectMapper.Map<BookingPayments>(entity.BookingPayment), db);
                    return bookingId;
                });
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

        [Authorize(Roles = nameof(UserRolesEnum.Administrator))]
        [HttpDelete("{id}")]
        public override async Task Delete(long id)
        {
            try
            {
                await base.Delete(id);
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, id);
                throw;
            }
        }



        [Authorize(Roles = nameof(UserRolesEnum.Administrator))]
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

        [HttpPost("/api/[controller]/HasCoachesNotOverlappedBooking")]
        public async Task<CheckResultDto> HasCoachesNotOverlappedBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToCoachesLinks)
                {
                    var check = await repository.HasCoachNotOverlappedBooking(link.Coach, booking);
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorsesNotOverlappedBooking")]
        public async Task<CheckResultDto> HasHorsesNotOverlappedBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);                
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToHorsesLinks)
                {
                    var check = await repository.HasHorseNotOverlappedBooking(link.Hor, booking);
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorsesRequiredBreak")]
        public async Task<CheckResultDto> HasHorsesRequiredBreak([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToHorsesLinks)
                {
                    var check = await repository.HasHorseRequiredBreak(link.Hor, booking);
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorsesWorkedLessThanAllowed")]
        public async Task<CheckResultDto> HasHorsesWorkedLessThanAllowed([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToHorsesLinks)
                {
                    var check = await repository.HasHorseWorkedLessThanAllowed(link.Hor, booking);
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasHorsesScheduleFitBooking")]
        public async Task<CheckResultDto> HasHorsesScheduleFitBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToHorsesLinks)
                {
                    var check = await repository.HasHorseScheduleFitBooking(link.Hor, booking);
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasCoachesScheduleFitBooking")]
        public async Task<CheckResultDto> HasCoachesScheduleFitBooking([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToCoachesLinks)
                {
                    var check = await repository.HasCoachScheduleFitBooking(link.Coach, booking); ;
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }

            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }

        [HttpPost("/api/[controller]/HasCoachScheduleFitBreaks")]
        public async Task<CheckResultDto> HasCoachScheduleFitBreaks([FromBody] BookingDto entity)
        {
            try
            {
                var booking = ObjectMapper.Map<Booking>(entity);
                var result = new CheckResultDto();

                foreach (var link in booking.BookingsToCoachesLinks)
                {
                    var check = await repository.HasCoachScheduleFitBreaks(link.Coach, booking); ;
                    if (!check.Result)
                    {
                        result.ErrorMessage += check.ErrorMessage;
                    }
                }

                return result;
            }

            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message, entity);
                throw;
            }
        }
    }
}
