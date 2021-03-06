﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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
        public static void SetupUiCommands<T, T1>(BaseReferenceVm<T, T1> vm, DataGrid dataGrid, int columnIndex = 0, bool readOnlyMode = true)
            where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
            where T : IUniqueDto, new()
        {
            if (readOnlyMode)
            {
                dataGrid.IsReadOnly = true;
                dataGrid.RowEditEnding += DataGrid_RowEditEnding;
            }

            vm.BeginEditItemCommand = new Command<object>((param) =>
            {
                dataGrid.Dispatcher.BeginInvoke(new DispatcherOperationCallback((p) =>
                {
                    dataGrid.IsReadOnly = false;
                    dataGrid.EditItemEventHandler(vm.SelectedItem, null, columnIndex);
                    return null;
                }), DispatcherPriority.Background, new object[] { null });
                
            }, (x) => vm.CanEditSelectedItem);

            vm.BeginAddItemCommand = new Command<object>((param) =>
            {
                dataGrid.IsReadOnly = false;
                dataGrid.SelectedItem = vm.AddEmptyItem();
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);

                dataGrid.Dispatcher.BeginInvoke(new DispatcherOperationCallback((p) =>
                {                                        
                    dataGrid.EditItemEventHandler(vm.SelectedItem, null, columnIndex);
                    return null;
                }), DispatcherPriority.Background, new object[] { null });
                
            }, (x) => vm.CanAddItem);            
        }

        private static void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if ((e.Row?.Item as IDataErrorInfo) != null && string.IsNullOrEmpty((e.Row?.Item as IDataErrorInfo).Error))
            {
                (sender as DataGrid).Dispatcher.BeginInvoke(new DispatcherOperationCallback((param) =>
                {
                    (sender as DataGrid).IsReadOnly = true;
                    return null;
                }), DispatcherPriority.Background, new object[] { null });
            }
        }
    }
}
