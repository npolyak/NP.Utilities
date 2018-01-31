using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Behaviors
{
    public abstract class ForEachItemCollectionBehaviorBase<T> :
        IStatelessBehavior<IEnumerable<T>>
    {
        protected abstract void UnsetItem(T item);
        protected abstract void SetItem(T item);

        private void UnsetItems(IEnumerable items)
        {
            if (items == null)
                return;

            foreach (T item in items)
            {
                UnsetItem(item);
            }
        }

        private void SetItems(IEnumerable items)
        {
            if (items == null)
                return;

            foreach (T item in items)
            {
                SetItem(item);
            }
        }


        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UnsetItems(e.OldItems);
            SetItems(e.NewItems);
        }

        public void Detach(IEnumerable<T> collection)
        {
            if (collection == null)
                return;

            INotifyCollectionChanged notifiableCollection =
                collection as INotifyCollectionChanged;

            if (notifiableCollection != null)
            {
                notifiableCollection.CollectionChanged -= Collection_CollectionChanged;
            }

            UnsetItems(collection);
        }

        public void Attach(IEnumerable<T> collection)
        {
            if (collection == null)
                return;

            SetItems(collection);

            INotifyCollectionChanged notifiableCollection =
                collection as INotifyCollectionChanged;

            if (notifiableCollection != null)
            {
                notifiableCollection.CollectionChanged += Collection_CollectionChanged;
            }
        }
    }
}
