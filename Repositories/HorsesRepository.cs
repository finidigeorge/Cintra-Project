using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class HorsesRepository: GenericRepository<Hors>
    {        
    }
}
