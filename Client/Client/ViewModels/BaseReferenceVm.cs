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
using Client.Annotations;
using Client.Commands;
using Client.ViewModels.Interfaces;
using Common;
using Common.DtoMapping;
using RestClient;
using Shared;
using Shared.Dto.Interfaces;
using Shared.Interfaces;

namespace Client.ViewModels
{    

    /*public class CustomObservableCollection<T> : ObservableCollection<T> where T : IUniqueDto, IAtomicEditableObject, new()
    {
        public CustomObservableCollection()
        {
        }

        public CustomObservableCollection(IEnumerable<T> values) : base(values)
        {
            foreach (var item in Items)
            {
                item.ItemEndEdit += ItemEndEditHandler;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            // handle any EndEdit events relating to this item
            item.ItemEndEdit += ItemEndEditHandler;
        }

        void ItemEndEditHandler(IAtomicEditableObject sender)
        {
            ItemEndEdit?.Invoke(sender);            
        }

        public event ItemEndEditEventHandler ItemEndEdit;
    }*/

    public class BaseReferenceVm<T, T1> : BaseVm, IEditableSelectableReference<T1> 
        where T1 : T, IUniqueDto, INotifyPropertyChanged, IAtomicEditableObject, new()
        where T: IUniqueDto, new()
    {
        private ObservableCollection<T1> _items;

        public CollectionView ItemsCollectionView { get; private set; }

        protected IBaseController<T> Client;

        protected BaseReferenceVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                Items = new ObservableCollection<T1>();
                foreach (var item in (await Client.GetAll()).ToList<T, T1>())                
                    Items.Add(item);                
                
            });

            AddItemCommand = new AsyncCommand<T1>(async (x) =>
            {
                var id = await Client.Insert(x);
                var item = await Client.GetById(id);
                Items[Items.IndexOf(Items.First(v => v.Id == 0))] = (T1) item;
            });

            EditItemCommand = new AsyncCommand<T1>(async (x) =>
            {
                await Client.Update(x);                                
            });

            DeleteItemCommand = new AsyncCommand<T1>(async (x) =>
            {
                await Client.Delete(x);                
            });

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
                            EditItemCommand.ExecuteAsync(obj);
                    };

                    //handler for tracking changes of particular properties of each item
                    ((INotifyPropertyChanged) x).PropertyChanged += (obj, args) => { EditItemCommand.ExecuteAsync(obj); };
                }
            }

            //delete from collection handler
            if (action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {
                foreach (var x in e.OldItems)
                {                    
                    DeleteItemCommand.ExecuteAsync(x);
                }
            }

            ItemsCollectionView.Refresh();
        }        

        public ObservableCollection<T1> Items
        {
            get => _items;
            set
            {                 
                Set(ref _items, value, nameof(Items));
                _items.CollectionChanged += OnCollectionChanged;                                         

                if (ItemsCollectionView == null)
                    ItemsCollectionView = new CollectionView(_items);

                ItemsCollectionView.Refresh();                
            }
        }
        

        public IAsyncCommand GetItemsCommand { get; set; }
        public IAsyncCommand AddItemCommand { get; set; }
        public IAsyncCommand DeleteItemCommand { get; set; }
        public IAsyncCommand EditItemCommand { get; set; }
        public IList<T1> SelectedItems { get; protected set; }
        public bool IsEditModeEnabled { get; protected set; }
        public bool IsSelectionModeEnabled { get; protected set; }
        public bool IsMultiSelectionModeEnabled { get; protected set; }
                        
    }
}
