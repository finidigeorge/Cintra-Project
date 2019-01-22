using DataModels;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingTemplatesMetadataRepository: IGenericRepository<BookingsTemplateMetadata>
    {
        Task CancelAllBookings(long metadataId, DateTime onDate, DataConnection dbContext = null);
        Task GenerateAllPermanentBookings(DateTime onDate, DataConnection dbContext = null);
    }
}
