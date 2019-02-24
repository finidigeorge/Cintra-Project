using Shared.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Filter
{
    public interface IFilterDefinition<T>
    {
        string SearchExpression { get; set; }
        bool IsDefined { get; }
        bool IsSatisfiedBy(T item);
        void Reset();
    }
}
