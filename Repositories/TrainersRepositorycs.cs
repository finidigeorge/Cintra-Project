﻿using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using Repositories.Interfaces;
using Shared.Attributes;

namespace Repositories
{
    [PerScope]
    public class TrainersRepositorycs: GenericRepository<Trainer>, ITrainersRepository
    {
    }
}
