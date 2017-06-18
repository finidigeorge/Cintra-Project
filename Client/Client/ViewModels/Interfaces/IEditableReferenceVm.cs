using Client.Commands;

namespace Client.ViewModels.Interfaces
{
    public interface IEditableReferenceVm
    {
        IAsyncCommand AddItemsCommand { get; set; }
        IAsyncCommand DeleteItemsCommand { get; set; }
        IAsyncCommand EditItemCommand { get; set; }
    }
}