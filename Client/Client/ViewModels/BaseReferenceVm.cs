﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Client.Annotations;
using Client.Commands;
using Client.ViewModels.Interfaces;

namespace Client.ViewModels
{
    public class BaseReferenceVm<T> : INotifyPropertyChanged, IEditableSelectableReference<T> where T : new()
    {
        private IList<T> _items = new List<T>();
   
        protected CollectionView ItemsCollectionView;

        public IList<T> Items
        {
            get => _items;
            set
            {                 
                Set(ref _items, value, "Items");
                if (ItemsCollectionView == null)
                    ItemsCollectionView = new CollectionView(_items);
            }
        }
        
        public IAsyncCommand GetItemsCommand { get; set; }
        public IAsyncCommand AddItemsCommand { get; set; }
        public IAsyncCommand DeleteItemsCommand { get; set; }
        public IAsyncCommand EditItemCommand { get; set; }
        public IList<T> SelectedItems { get; protected set; }
        public bool IsEditModeEnabled { get; protected set; }
        public bool IsSelectionModeEnabled { get; protected set; }
        public bool IsMultiSelectionModeEnabled { get; protected set; }
        

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Set<T1>(ref T1 oldValue, T1 newValue, string propertyName)
        {
            if (!oldValue.Equals(newValue))
            {
                oldValue = newValue;
                OnPropertyChanged(propertyName);
            }
        }
    }
}