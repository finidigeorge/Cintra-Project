using Common.DtoMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Filter
{
    public class HorseSearchFilter : BaseVm, IFilterDefinition<HorseDtoUi>
    {
        public string SearchExpression { get; set; }
        public bool IsDefined => !string.IsNullOrEmpty(SearchExpression);

        public bool IsSatisfiedBy(HorseDtoUi item)
        {
            return !IsDefined || item.NickName.ToLowerInvariant().Contains(SearchExpression.ToLowerInvariant());
        }

        public void Reset()
        {
            SearchExpression = string.Empty;
        }
    }
}
