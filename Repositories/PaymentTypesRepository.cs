using DataModels;
using Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    [PerScope]
    public class PaymentTypesRepository: GenericPreservableRepository<PaymentTypes>
    {
    }
}
