using DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingTemplatesMetadataRepository: IGenericRepository<BookingsTemplateMetadata>
    {
        Task CancelAllBookings(long metadataId, DateTime onDate, CintraDB dbContext = null);
        Task GenerateAllPermanentBookings(DateTime onDate, CintraDB dbContext = null);
    }
}
