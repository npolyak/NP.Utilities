using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Behaviors
{
    public class DoForEachItemCollectionBehavior<T> : ForEachItemCollectionBehaviorBase<T>
    {
        Action<T> UnsetItemDelegate { get; }
        Action<T> SetItemDelegate { get; }

        protected override void UnsetItem(T item)
        {
            UnsetItemDelegate?.Invoke(item);
        }

        protected override void SetItem(T item)
        {
            SetItemDelegate?.Invoke(item);
        }

        public DoForEachItemCollectionBehavior(Action<T> OnAdd, Action<T> OnRemove = null)
        {
            SetItemDelegate = OnAdd;
            UnsetItemDelegate = OnRemove;
        }
    }

    public static class DoForEachBehaviorUtils
    {
        private static BehaviorsDisposable<IEnumerable<T>> AddBehaviorImpl<T>
        (
            this IEnumerable<T> collection,
            DoForEachItemCollectionBehavior<T> behavior,
            BehaviorsDisposable<IEnumerable<T>> previousBehavior = null
        )
        {
            if (collection == null)
                return null;

            DisposableBehaviorContainer<IEnumerable<T>> behaviorContainer =
                new DisposableBehaviorContainer<IEnumerable<T>>(behavior, collection);

            BehaviorsDisposable<IEnumerable<T>> behaviorsDisposable =
                new BehaviorsDisposable<IEnumerable<T>>(behaviorContainer, previousBehavior);

            return behaviorsDisposable;
        }

        private static BehaviorsDisposable<IEnumerable<T>> AddBehaviorImpl<T>
        (
            this IEnumerable<T> collection,
            Action<T> onAdd,
            Action<T> onRemove = null,
            BehaviorsDisposable<IEnumerable<T>> previousBehavior = null
        )
        {
            DoForEachItemCollectionBehavior<T> behavior = new DoForEachItemCollectionBehavior<T>(onAdd, onRemove);

            return collection?.AddBehaviorImpl<T>(behavior, previousBehavior);
        }

        public static BehaviorsDisposable<IEnumerable<T>> AddBehavior<T>
        (
            this IEnumerable<T> collection,
            DoForEachItemCollectionBehavior<T> behavior
        )
        {
            return collection.AddBehaviorImpl<T>(behavior);
        }


        public static BehaviorsDisposable<IEnumerable<T>> AddBehavior<T>
        (
            this IEnumerable<T> collection, 
            Action<T> onAdd,
            Action<T> onRemove = null
        )
        {
            return collection.AddBehaviorImpl<T>(onAdd, onRemove);
        }

        public static BehaviorsDisposable<IEnumerable<T>> AddBehavior<T>
        (
            this BehaviorsDisposable<IEnumerable<T>> previousBehaviors,
            Action<T> onAdd,
            Action<T> onRemove = null
        )
        {
            IEnumerable<T> collection = previousBehaviors.TheObjectTheBehaviorsAreAttachedTo;

            return collection.AddBehaviorImpl<T>(onAdd, onRemove, previousBehaviors);
        }
    }
}
