using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Behaviors
{
    public class ParentChildSelectionBehavior<TParent, TChild>
        where TParent : class, ISelectableItem<TParent>
        where TChild : class, ISelectableItem<TChild>
    {
        IDisposable _childrenBehaviorDisposable = null;

        TParent _parent;
        public TParent Parent
        {
            get => _parent;

            set
            {
                if (_parent.ObjEquals(value))
                    return;

                if (_parent != null)
                {
                    _parent.IsSelectedChanged -=
                        ParentChildSelectionBehavior_IsSelectedChanged;
                }

                _parent = value;

                if (_parent != null)
                {
                    _parent.IsSelectedChanged +=
                        ParentChildSelectionBehavior_IsSelectedChanged;
                }
            }
        }

        ObservableCollection<TChild> _children;
        public ObservableCollection<TChild> Children
        {
            private get => _children;
            set
            {
                if (ReferenceEquals(_children, value))
                    return;

                _children = value;

                _childrenBehaviorDisposable?.Dispose();
                _childrenBehaviorDisposable = _children.AddBehavior
                 (
                    child => child.IsSelectedChanged += Child_IsSelectedChanged,
                    child => child.IsSelectedChanged -= Child_IsSelectedChanged
                 );
            }
        }

        // unselect children if parent is unselected
        private void ParentChildSelectionBehavior_IsSelectedChanged(ISelectableItem<TParent> parent)
        {
            if (!parent.IsSelected)
            {
                foreach(TChild child in this.Children)
                {
                    if (child.IsSelected)
                    {
                        child.IsSelected = false;
                    }
                }
            }
        }

        // select the parent if its child is selected. 
        private void Child_IsSelectedChanged(ISelectableItem<TChild> child)
        {
            if ((child.IsSelected) && (this.Parent != null))
            {
                this.Parent.IsSelected = true;
            }
        }
    }
}
