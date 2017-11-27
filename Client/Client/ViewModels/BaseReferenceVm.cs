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
using WPFCustomMessageBox;

namespace Client.ViewModels
{    
    public class BaseReferenceVm<T, T1> : BaseVm, IEditableSelectableReference<T1> 
        where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
        where T: IUniqueDto, new()
        
    {                

        public CollectionView ItemsCollectionView { get; private set; }

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

                if (ItemsCollectionView == null)
                    ItemsCollectionView = new ListCollectionView(_items);

                ItemsCollectionView.Refresh();
            }
        }

        public IBaseController<T> Client;

        protected virtual async Task<IList<T>> GetItems()
        {
            return await Client.GetAll();
        }

        protected BaseReferenceVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                Items = new ObservableCollection<T1>();
                foreach (var item in (await GetItems()).ToList<T, T1>())                
                    Items.Add(item);

                ItemsCollectionView?.Refresh();

            });

            RefreshDataCommand = new AsyncCommand<object>(async (x) =>
            {
                await GetItemsCommand.ExecuteAsync(x);
            });

            AddItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                BeforeAddItemHandler(param);
                var id = await Client.Create(ObjectMapper.Map<T>(param));
                var item = ObjectMapper.Map<T1>(await Client.GetById(id));

                //two cases for scheduler and table models
                if (Items.Any(v => v.Id == 0))
                    Items[Items.IndexOf(Items.First(v => v.Id == 0))] = item;
                else
                    Items.Add(item);
            }, (x) => x != null);

            UpdateItemCommand = new AsyncCommand<T1>(async (param) =>
            {
                BeforeEditItemHandler(param);
                await Client.Create(ObjectMapper.Map<T>(param));                
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
                if (SelectedItem != null)
                    Items.Remove(Items.First(i => i.Id == SelectedItem.Id));                
            }, (x) => CanDeleteSelectedItem);


            BeginDeleteItemCommand = new Command<object>(() =>
            {
                if (CustomMessageBox.Show(Messages.DELETE_RECODRD_CONFIRM_MSG, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DeleteSelectedItemCommand.Execute(SelectedItem);
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

                    //remove new item on cancel edit
                    ((IAtomicEditableObject)x).ItemCancelEdit += (obj) =>
                    {
                        if (((IUniqueDto) obj).Id == 0)
                            Items.Remove((T1) obj);                        
                    };                    
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

        //command-related event handlers
        protected virtual void BeforeAddItemHandler(T1 item)
        {
        }

        protected virtual void BeforeEditItemHandler(T1 item)
        {
        }

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
        public ICommand BeginDeleteItemCommand { get; set; }


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
