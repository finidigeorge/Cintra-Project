using System.Collections.Generic;
using Client.Commands;

namespace Client.ViewModels.Interfaces
{
    public interface IReferenceVm<T> where T: new()
    {
        IList<T> Items { get; }
        IAsyncCommand GetItemsCommand { get; set; }        
    }
}