using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Client.Commands;
using Client.ViewModels;
using Common;
using Common.DtoMapping;
using Shared.Dto.Interfaces;
using WPFCustomMessageBox;

namespace Client.Extentions
{
    public class ReferenceVmHelper
    {
        public static void SetupUiCommands<T, T1>(BaseReferenceVm<T, T1> vm, DataGrid dataGrid)
            where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
            where T : IUniqueDto, new()
        {
            vm.BeginEditItemCommand = new Command<object>(() =>
            {
                dataGrid.EditItemEventHandler(vm.SelectedItem, null);
            }, (x) => vm.CanEditSelectedItem);

            vm.BeginAddItemCommand = new Command<object>(() =>
            {
                dataGrid.SelectedItem = vm.AddEmptyItem();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);

                vm.BeginEditItemCommand.Execute(dataGrid);
            }, (x) => vm.CanAddItem);

            vm.BeginDeleteItemCommand = new Command<object>(() =>
            {
                if (CustomMessageBox.Show(Messages.DELETE_RECODRD_CONFIRM_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    vm.DeleteSelectedItemCommand.ExecuteAsync(vm.SelectedItem);
                }

            }, (x) => vm.CanDeleteSelectedItem);
        }
    }
}
