using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Interfaces
{
    public interface IEditableSelectableReference<T> : IEditableReferenceVm, ISelectableReference<T> where T: new()
    {
        bool IsEditModeEnabled { get; }
        bool IsSelectionModeEnabled { get; }
        bool IsMultiSelectionModeEnabled { get; }
    }
}
