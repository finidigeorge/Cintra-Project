using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Client.Annotations;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common;
using Common.DtoMapping;
using Mapping;
using PropertyChanged;
using RestClient;
using Shared;
using Shared.Dto.Interfaces;
using Shared.Interfaces;

namespace Client.ViewModels
{        
    public class BaseReferenceVm<T, T1> : BaseVm, IEditableSelectableReference<T1> 
        where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
        where T: IUniqueDto, new()
        
    {                

        public CollectionView ItemsCollectionView { get; private set; }        

        public T1 SelectedItem { get; set; }

        private ObservableCollection<T1> _items;

        //manula implementation of NotifyPropertyChanged
        [DoNotNotify]
        public ObservableCollection<T1> Items
        {
            get => _items;
            set
            {
                Set(ref _items, value, nameof(Items));
                _items.CollectionChanged += OnCollectionChanged;

                if (ItemsCollectionView == null)
                    ItemsCollectionView = new ListCollectionView(_items);

                ItemsCollectionView.Refresh();
            }
        }

        protected IBaseController<T> Client;

        protected BaseReferenceVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                Items = new ObservableCollection<T1>();
                foreach (var item in (await Client.GetAll()).ToList<T, T1>())                
                    Items.Add(item);

                ItemsCollectionView?.Refresh();

            });

            AddItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                var id = await Client.Insert(param);
                var item = ObjectMapper.Map<T1>(await Client.GetById(id));
                Items[Items.IndexOf(Items.First(v => v.Id == 0))] = item;                
            }, (x) => x != null);

            UpdateItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await Client.Update(param);                
            }, (x) => x != null);

            DeleteItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await Client.Delete(param.Id);                
            }, (x) => x != null);

            UpdateSelectedItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await UpdateItemCommand.ExecuteAsync(SelectedItem);
                var idx = Items.IndexOf(Items.First(i => i.Id == SelectedItem.Id));
                Items[idx] = SelectedItem;

                ItemsCollectionView.Refresh();

            }, (x) => CanEditSelectedItem);

            DeleteSelectedItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await DeleteItemCommand.ExecuteAsync(SelectedItem);
                Items.Remove(Items.First(i => i.Id == SelectedItem.Id));                
            }, (x) => CanDeleteSelectedItem);

            Items = new ObservableCollection<T1>();            
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedAction action = e.Action;

            if (action == NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
            {
                foreach (var x in e.NewItems)
                {
                    //handler for tracking changes in the collection done by DataGrid
                    ((IAtomicEditableObject) x).ItemEndEdit += (obj) =>
                    {
                        if (((IUniqueDto)obj).Id == 0)
                            AddItemCommand.ExecuteAsync(obj);
                        else
                            UpdateItemCommand.ExecuteAsync(obj);
                    };

                    //handler for tracking changes of particular properties of each item
                    ((INotifyPropertyChanged) x).PropertyChanged += (obj, args) => { UpdateItemCommand.ExecuteAsync(obj); };
                }
            }

            //delete from collection handler
            if (action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {                
                ItemsCollectionView.Refresh();
            }            
        }

        public T1 AddEmptyItem()
        {
            var newItem = new T1();
            Items.Add(newItem);
            return newItem;
        }


        //predefined Back end related Commands 
        public IAsyncCommand GetItemsCommand { get; set; }
        public IAsyncCommand AddItemCommand { get; set; }
        public IAsyncCommand DeleteItemCommand { get; set; }
        public IAsyncCommand DeleteSelectedItemCommand { get; set; }
        public IAsyncCommand UpdateItemCommand { get; set; }
        public IAsyncCommand UpdateSelectedItemCommand { get; set; }

       
        //UI Event wrappers commands
        public ICommand BeginEditItemCommand { get; set; }
        public ICommand BeginAddItemCommand { get; set; }

        
        public bool IsEditModeEnabled { get; protected set; } = true;

        public bool CanAddItem => IsEditModeEnabled;
        public bool CanEditSelectedItem => IsEditModeEnabled && SelectedItem != null;
        public bool CanDeleteSelectedItem => IsEditModeEnabled && SelectedItem != null;

        //not implemented yet
        public IList<T1> SelectedItems { get; protected set; }

       
        public bool IsSelectionModeEnabled { get; protected set; }
        public bool IsMultiSelectionModeEnabled { get; protected set; }
                        
    }
}
