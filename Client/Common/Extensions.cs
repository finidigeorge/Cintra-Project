using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mapping;

namespace Common
{
    public static class Extensions
    {
        public static IList<T1> ToList<T, T1>(this IEnumerable<T> value)
        {
            return value.Select(x => ObjectMapper.Map<T1>(x)).ToList();
        }

        public static IList<T1> ToList<T, T1>(this IList<T> value)
        {
            return value.Select(x => ObjectMapper.Map<T1>(x)).ToList();
        }       
    
        public static void NotifyPropertyChanged<T>(this ObservableCollection<T> observableCollection, Action<T, PropertyChangedEventArgs> callBackAction)
            where T : INotifyPropertyChanged
        {
            observableCollection.CollectionChanged += (sender, args) =>
            {                
                if (args.NewItems == null) return;
                foreach (T item in args.NewItems)
                {
                    item.PropertyChanged += (obj, eventArgs) =>
                    {
                        callBackAction((T)obj, eventArgs);
                    };
                }
            };
        }
    }
}
