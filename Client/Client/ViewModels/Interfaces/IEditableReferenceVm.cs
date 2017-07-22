using Client.Commands;

namespace Client.ViewModels.Interfaces
{
    public interface IEditableReferenceVm
    {
        IAsyncCommand AddItemCommand { get; set; }
        IAsyncCommand DeleteItemCommand { get; set; }
        IAsyncCommand EditItemCommand { get; set; }
    }
}