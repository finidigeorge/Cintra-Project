using DataModels;
using LinqToDB;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    [PerScope]
    public class ClientsRepository: GenericPreservableRepository<Client>
    {
        public override async Task<List<Client>> GetByParams(Expression<Func<Client, bool>> where, CintraDB dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {
                return await Task.FromResult(db.GetTable<Client>()
                    .Where(where)
                    .Where((x) => x.IsDeleted == false)
                    .OrderBy(x => x.Name)
                    .ToList());
            }, dbContext);
        }
    }
}
