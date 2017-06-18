using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Interfaces
{
    public interface ISelectableReference<T> where T: new()
    {
        IList<T> SelectedItems { get; }
    }
}
