using DataModels;
using LinqToDB;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class ClientsRepository: GenericPreservableRepository<Client>
    {
        public override async Task<List<Client>> GetByParams(Func<Client, bool> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return (await base.GetByParams(where, db)).OrderBy(x => x.Name);
            }, dbContext);
        }
    }
}
