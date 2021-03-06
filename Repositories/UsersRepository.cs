﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Repositories.Interfaces;
using Shared;
using Shared.Attributes;
using Shared.Dto;

namespace Repositories
{
    [PerScope]
    public class UserRepository : GenericPreservableRepository<User>, IUsersRepository
    {
        public override async Task<List<User>> GetByParams(Expression<Func<User, bool>> where, DataConnection dbContext = null)
        {
            return await RunWithinTransaction(async (db) =>
            {                
                return await Task.FromResult(
                    db.GetTable<User>().LoadWith(x => x.UserRole).Where(where).Where(x => x.IsDeleted == false).ToList()
                );
            }, dbContext);
        }
        
        public async Task<User> GetByLogin(string login)
        {
            using (var db = new DataConnection())
            {
                return (await GetByParams(x => x.Login == login)).FirstOrDefault();
            }
        }

        public async Task<bool> HasAdminAccess(string login)
        {
            var user = await GetByLogin(login);
            return user?.UserRole.Name == nameof(UserRolesEnum.Administrator);
        }
    }
}
