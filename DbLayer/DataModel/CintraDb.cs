using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;
using DbLayer.Interfaces;
using LinqToDB.Data;

namespace DataModels
{
    public partial class CintraDB
    {
        partial void InitDataContext()
        {
            DataConnectionExtensions.Execute(this, "PRAGMA foreign_keys = ON");
        }
    }

    public partial class Booking : IPreservable { }
    public partial class BookingPayments : IPreservable { }
    public partial class Coach : IPreservable {}
    public partial class Client : IPreservable { }
    public partial class Hors : IPreservable { }
    public partial class Schedule : IPreservable { }
    public partial class SchedulesData : IPreservable { }
    public partial class SchedulesInterval : IUniqueDto { }
    public partial class Service : IPreservable { }
    public partial class User : IPreservable { }
    public partial class UserRoles : IPreservable { }
}
