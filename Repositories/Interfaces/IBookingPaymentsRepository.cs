﻿using DataModels;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingPaymentsRepository: IGenericRepository<BookingPayments>
    {
        Task SynchronizeWithBooking(long bookingId, BookingPayments payment, DataConnection dbContext = null);
    }
}
