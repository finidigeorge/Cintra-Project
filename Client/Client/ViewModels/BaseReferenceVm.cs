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
using RestClient;
using Shared;
using Shared.Dto.Interfaces;
using Shared.Interfaces;

namespace Client.ViewModels
{
    public class BaseReferenceVm<T, T1> : BaseVm, IEditableSelectableReference<T1> 
        where T1 : T, IUniqueDto, INotifyPropertyChanged, new()
        where T: IUniqueDto, new()
    {
        private ObservableCollection<T1> _items = new ObservableCollection<T1>();

        public CollectionView ItemsCollectionView { get; private set; }

        protected IBaseController<T> Client;

        protected BaseReferenceVm()
        {
            GetItemsCommand = new AsyncCommand<object>(async (x) =>
            {
                Items = new ObservableCollection<T1>((await Client.GetAll()).ToList<T, T1>());
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
                Items[Items.IndexOf(Items.First(v => v.Id == x.Id))] = x;                
            });

            DeleteItemCommand = new AsyncCommand<T1>(async (x) =>
            {
                await Client.Delete(x);                
            });

            Items.CollectionChanged += OnCollectionChanged;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedAction action = e.Action;

            if (action == NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
            {
                if (e.NewItems.Count > 1)
                    throw new Exception("Multiple inserts are not supported");

                AddItemCommand.ExecuteAsync(e.NewItems[0]);
            }

            if (action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {
                foreach (var x in e.OldItems)
                    EditItemCommand.ExecuteAsync(x);
            }

            if (action == NotifyCollectionChangedAction.Replace && e.OldItems?.Count > 0)
            {
                foreach (var x in e.OldItems)
                    DeleteItemCommand.ExecuteAsync(x);                
            }            
        }        

        public ObservableCollection<T1> Items
        {
            get => _items;
            set
            {                 
                Set(ref _items, value, nameof(Items));
                if (ItemsCollectionView == null)
                    ItemsCollectionView = new CollectionView(_items);
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
