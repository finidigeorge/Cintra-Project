using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Repositories.Interfaces;

namespace Repositories
{
    public class HorsesRepository: GenericRepository<Hors>, IHorsesRepository
    {
    }
}
