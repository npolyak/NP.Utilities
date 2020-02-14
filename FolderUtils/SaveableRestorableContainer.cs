using System;
using System.ComponentModel;

namespace NP.Utilities.FolderUtils
{
    public class SaveableRestorableContainer<T> :
        ItemToFolderSaverRestorer,
        INotifyPropertyChanged
        where T : new()
    {
        public virtual void Reset()
        {
            this.TheItemLocationName = null;
            this.TheRestorableSaveableItem = new T();
        }

        public SaveableRestorableContainer(string folderPath) :
            base(folderPath)
        {
            TheRestorableSaveableItem = new T();
        }

        public event Action RestorableSaveableItemChangedEvent = null;

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #region TheRestorableSaveableItem Property
        private T _restorableSaveableItem;
        public T TheRestorableSaveableItem
        {
            get
            {
                return this._restorableSaveableItem;
            }
            set
            {
                if (this._restorableSaveableItem.ObjEquals(value))
                {
                    return;
                }

                this._restorableSaveableItem = value;
                this.OnPropertyChanged(nameof(TheRestorableSaveableItem));

                this.RestorableSaveableItemChangedEvent?.Invoke();
            }
        }
        #endregion TheRestorableSaveableItem Property


        #region TheItemLocationName Property
        private string _itemLocationName;
        public string TheItemLocationName
        {
            get
            {
                return this._itemLocationName;
            }
            set
            {
                if (this._itemLocationName == value)
                {
                    return;
                }

                this._itemLocationName = value;
                this.OnPropertyChanged(nameof(TheItemLocationName));
            }
        }
        #endregion TheItemLocationName Property


        //public virtual void Save()
        //{
        //    SerializeAndSave(TheRestorableSaveableItem, TheItemLocationName);
        //}

        //public virtual void Restore()
        //{
        //    T result = this.RestoreAndDeserialize<T>(TheItemLocationName);

        //    if (result != null)
        //        TheRestorableSaveableItem = result;
        //    else
        //        TheRestorableSaveableItem = new T();
        //}
    }
}
