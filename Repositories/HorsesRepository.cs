using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using Repositories.Interfaces;
using Shared.Attributes;
using Shared.Extentions;

namespace Repositories
{
    [PerScope]
    public class HorsesRepository: GenericPreservableRepository<Hors>
    {                        
        public override async Task<List<Hors>> GetByParams(Func<Hors, bool> where)
        {
            using (var db = new CintraDB())
            {
                return await Task.FromResult(
                    db.Horses
                        .LoadWith(x => x.Bookings)
                        .LoadWith(x => x.HorsesScheduleData)
                        .LoadWith(x => x.ServiceToHorsesLinks)
                        .Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }
        }        
    }
}
