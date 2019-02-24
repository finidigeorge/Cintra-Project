using Common.DtoMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Filter
{
    public class ClientSearchFilter : BaseVm, IFilterDefinition<ClientDtoUi>
    {
        public string SearchExpression { get; set; }
        public bool IsDefined => !string.IsNullOrEmpty(SearchExpression);

        public bool IsSatisfiedBy(ClientDtoUi item)
        {
            return !IsDefined || item.Name.ToLowerInvariant().Contains(SearchExpression.ToLowerInvariant());
        }

        public void Reset()
        {
            SearchExpression = string.Empty;
        }
    }
}

