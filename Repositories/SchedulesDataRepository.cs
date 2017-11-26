using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class SchedulesDataRepository : GenericPreservableRepository<SchedulesData>
    {
    }    
}
