using System;
using System.Collections.Generic;
using System.Text;
using Shared.Dto.Interfaces;

namespace DataModels
{
    public partial class Coach : IUniqueDto {}
    public partial class Hors : IUniqueDto { }
    public partial class Schedule : IUniqueDto { }
    public partial class SchedulesData : IUniqueDto { }
    public partial class SchedulesInterval : IUniqueDto { }
    public partial class Service : IUniqueDto { }
    public partial class User : IUniqueDto { }
    public partial class UserRoles : IUniqueDto { }
}
