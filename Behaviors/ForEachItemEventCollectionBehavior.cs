using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Behaviors
{
    public class ForEachItemEventCollectionBehavior<T> : ForEachItemCollectionBehaviorBase<T>
    {
        public event Action<T> UnsetItemEvent;
        public event Action<T> SetItemEvent;

        protected override void UnsetItem(T item)
        {
            UnsetItemEvent.Invoke(item);
        }

        protected override void SetItem(T item)
        {
            SetItemEvent.Invoke(item);
        }
    }
}
