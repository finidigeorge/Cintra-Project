﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Client.Annotations;
using Client.Commands;
using Client.Security;
using Client.ViewModels.Filter;
using Client.ViewModels.Interfaces;
using Common;
using Common.DtoMapping;
using Mapping;
using PropertyChanged;
using RestClient;
using Shared;
using Shared.Dto.Interfaces;
using Shared.Interfaces;
using WPFCustomMessageBox;

namespace Client.ViewModels
{    
    public class BaseReferenceVm<T, T1> : BaseVm, IEditableSelectableReference<T1> 
        where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
        where T: IUniqueDto, new()
        
    {
        private static SemaphoreSlim _lockObject = new SemaphoreSlim(1, 1);

        private ICollectionView _itemsCollectionView;
        public ICollectionView ItemsCollectionView { get { return _itemsCollectionView; } }

        public event EventHandler<T1> OnSelectedItemChanged;
        public T1 SelectedItem { get; set; }
       
        private ObservableCollection<T1> _items;       

        //manual implementation of NotifyPropertyChanged
        [DoNotNotify]
        public ObservableCollection<T1> Items
        {
            get => _items;
            set
            {
                Set(ref _items, value, nameof(Items));
                _items.CollectionChanged += OnCollectionChanged;
                _itemsCollectionView = CollectionViewSource.GetDefaultView(_items);                
            }
        }

        public IBaseController<T> Client;

        protected virtual async Task<IList<T>> GetAll()
        {
            return await Client.GetAll();
        }

        public bool HasValidUser()
        {
            var principal = Thread.CurrentPrincipal as UserPrincipal;
            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
                return true;

            return false;
        }

        public bool HasAdminRights()
        {
            var principal = Thread.CurrentPrincipal as UserPrincipal;
            if (HasValidUser() && principal.IsInRole(nameof(UserRolesEnum.Administrator)))
                return true;

            return false;
        }

        #region Constructor
        protected BaseReferenceVm()
        {            
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                try
                {
                    await _lockObject.WaitAsync();

                    long selectedItemId = 0;
                    if (SelectedItem != null)
                        selectedItemId = SelectedItem.Id;

                    if (Items == null)
                        Items = new ObservableCollection<T1>();
                    else
                        Items.Clear();

                    foreach(var item in (await GetAll()).ToList<T, T1>())
                        Items.Add(item);

                    if (selectedItemId != 0)
                    {
                        SelectedItem = Items.FirstOrDefault(i => i.Id == selectedItemId);
                    }
                }
                finally {
                    _lockObject.Release();
                }

            }, (x) => HasValidUser());

            RefreshDataCommand = new AsyncCommand<object>(async (x) =>
            {
                await GetItemsCommand.ExecuteAsync(x);                
            }, (x) => HasValidUser());

            AddItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                BeforeAddItemHandler(param);

                await _lockObject.WaitAsync();
                try
                {
                    var id = await Client.Create(ObjectMapper.Map<T>(param));
                    var item = ObjectMapper.Map<T1>(await Client.GetById(id));

                    //two cases for scheduler and table models
                    if (Items.Any(v => v.Id == 0))
                        Items[Items.IndexOf(Items.First(v => v.Id == 0))] = item;
                    else
                        Items.Add(item);
                }
                finally
                {
                    _lockObject.Release();
                }

            }, (x) => x != null && HasValidUser());

            UpdateItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                BeforeEditItemHandler(param);

                await Client.Create(ObjectMapper.Map<T>(param));                
            }, (x) => x != null && HasValidUser());

            DeleteItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await Client.Delete(param.Id);                
            }, (x) => x != null && HasValidUser());

            UpdateSelectedItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await UpdateItemCommand.ExecuteAsync(SelectedItem);
                var idx = Items.IndexOf(Items.First(i => i.Id == SelectedItem.Id));
                Items[idx] = SelectedItem;                
            }, (x) => CanEditSelectedItem);

            DeleteSelectedItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                await _lockObject.WaitAsync();
                try
                {                    
                    await DeleteItemCommand.ExecuteAsync(SelectedItem);
                    if (SelectedItem != null)
                        Items.Remove(Items.First(i => i.Id == SelectedItem.Id));
                }
                finally
                {
                    _lockObject.Release();
                }

            }, (x) => CanDeleteSelectedItem);


            BeginDeleteItemCommand = new AsyncCommand<object>(async (param) =>
            {
                if (CustomMessageBox.Show(Messages.DELETE_RECODRD_CONFIRM_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await DeleteSelectedItemCommand.ExecuteAsync(SelectedItem);
                }

            }, (x) => CanDeleteSelectedItem);

            Items = new ObservableCollection<T1>();

            PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == nameof(SelectedItem))
                {
                    OnSelectedItemChanged?.Invoke(o, SelectedItem);
                }                
            };

            ClearSearchCommand = new AsyncCommand<object>(async (param) => 
            {
                Filter?.Reset();                
                await RefreshDataCommand.ExecuteAsync(param);
            }, (x) => true);
        }
        #endregion

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

                    //remove new item on cancel edit
                    ((IAtomicEditableObject)x).ItemCancelEdit += (obj) =>
                    {
                        if (((IUniqueDto) obj).Id == 0)
                            Items.Remove((T1) obj);                        
                    };                    
                }
            }

            //delete from collection handler
            /*if (action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {                
                ItemsCollectionView.Refresh();
                OnPropertyChanged(nameof(ItemsCollectionView));
            }  */          
        }

        public T1 AddEmptyItem()
        {
            var newItem = new T1();
            Items.Add(newItem);
            return newItem;
        }

        //command-related event handlers
        protected virtual void BeforeAddItemHandler(T1 item)
        {
        }

        protected virtual void BeforeEditItemHandler(T1 item)
        {
        }

        //not implemented yet
        public IList<T1> SelectedItems { get; protected set; }

        public bool IsSelectionModeEnabled { get; protected set; }
        public bool IsMultiSelectionModeEnabled { get; protected set; }

        #region Refernce Command defininions

        //predefined back end related Commands 
        public IAsyncCommand RefreshDataCommand { get; set; }
        public IAsyncCommand GetItemsCommand { get; set; }        
        public IAsyncCommand AddItemCommand { get; set; }
        public IAsyncCommand DeleteItemCommand { get; set; }
        public IAsyncCommand DeleteSelectedItemCommand { get; set; }
        public IAsyncCommand UpdateItemCommand { get; set; }
        public IAsyncCommand UpdateSelectedItemCommand { get; set; }

       
        //UI Event wrappers commands
        public ICommand BeginEditItemCommand { get; set; }
        public ICommand BeginAddItemCommand { get; set; }
        public IAsyncCommand BeginDeleteItemCommand { get; set; }


        public bool IsEditModeEnabled { get; protected set; } = true;

        public bool CanAddItem => IsEditModeEnabled && HasValidUser();
        public bool CanEditSelectedItem => IsEditModeEnabled && SelectedItem != null && HasValidUser();
        public bool CanDeleteSelectedItem => IsEditModeEnabled && SelectedItem != null && HasValidUser();

        #endregion

        #region Filter

        private IFilterDefinition<T1> _filter;
        public IFilterDefinition<T1> Filter {
            get => _filter;
            set
            {
                _filter = value;
                _itemsCollectionView.Filter = _filter != null ? new Predicate<object>(x => _filter.IsSatisfiedBy((T1)x)) : null;
            }
        }
        public IAsyncCommand ClearSearchCommand { get; set; }

        #endregion

    }
}
