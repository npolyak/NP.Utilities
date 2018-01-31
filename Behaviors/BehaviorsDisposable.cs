using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Behaviors
{
    public class BehaviorsDisposable<T> : IDisposable
    {
        List<DisposableBehaviorContainer<T>> _disposableBehaviors = new List<DisposableBehaviorContainer<T>>();


        public T TheObjectTheBehaviorsAreAttachedTo =>
            _disposableBehaviors.LastOrDefault().TheObjectTheBehaviorIsAttachedTo;

        public BehaviorsDisposable
        (
            DisposableBehaviorContainer<T> disposableBehaviorToAdd,
            BehaviorsDisposable<T> previousBehavior = null
        )
        {
            if (previousBehavior != null)
            {
                _disposableBehaviors.AddAll(previousBehavior._disposableBehaviors);
            }

            _disposableBehaviors.Add(disposableBehaviorToAdd);
        }

        public void Dispose()
        {
            foreach(DisposableBehaviorContainer<T> behaviorContainer in _disposableBehaviors)
            {
                behaviorContainer.Dispose();
            }
        }
    }
}
