using System.Collections.Specialized;
using System.ComponentModel;

namespace NP.Utilities.BasicInterfaces;

public interface ICollAndPropNotifiable : 
    INotifyPropertyChanged,
    INotifyCollectionChanged
{
}
