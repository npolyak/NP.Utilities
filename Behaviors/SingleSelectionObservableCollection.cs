using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NP.Utilities.Behaviors
{
    public class SingleSelectionObservableCollection<T, TSelect> : 
        ObservableCollection<T>,
        INotifyCollectionChanged
        where T : class, ISelectableItem<TSelect>
        where TSelect : class, ISelectableItem<TSelect>
    {
        public event Action SelectedItemChangedEvent = null;

        protected void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        #region TheSelectedItem Property
        private T _selectedItem;
        [XmlIgnore]
        public T TheSelectedItem
        {
            get
            {
                return this._selectedItem;
            }
            set
            {
                if (ReferenceEquals(_selectedItem, value))
                {
                    return;
                }

                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = false;
                }

                this._selectedItem = value;

                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = true;
                }

                SelectedItemChangedEvent?.Invoke();
                this.OnPropertyChanged(nameof(TheSelectedItem));
            }
        }
        #endregion TheSelectedItem Property


        public SingleSelectionObservableCollection()
        {
            this.CollectionChanged += 
                SingleSelectionObservableCollection_CollectionChanged;
        }

        private void SingleSelectionObservableCollection_CollectionChanged
        (
            object sender, 
            NotifyCollectionChangedEventArgs e
        )
        {
            UnsetItems(e.OldItems);
            SetItems(e.NewItems);
        }

        private void UnsetItems(IList oldItems)
        {
            if (oldItems == null)
                return;

            foreach(T item in oldItems)
            {
                item.IsSelectedChanged -= Item_IsSelectedChanged;

                if (ReferenceEquals(item, TheSelectedItem))
                {
                    TheSelectedItem = null;
                }
            }
        }


        private void SetItems(IList newItems)
        {
            if (newItems == null)
                return;

            foreach(T item in newItems)
            {
                item.IsSelectedChanged += Item_IsSelectedChanged;
            }
        }


        private void Item_IsSelectedChanged(ISelectableItem<TSelect> item)
        {
            if (item.IsSelected)
            {
                this.TheSelectedItem = (T) item;
            }
            else
            {
                TheSelectedItem = null;
            }
        }
    }

    public class SingleSelectionObservableCollection<T> :
        SingleSelectionObservableCollection<T, T>
        where T : class, ISelectableItem<T>
    {

    }
}
