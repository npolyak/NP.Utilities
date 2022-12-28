using System;

namespace NP.Utilities.PluginUtils
{
    public class ViewModelPluginInfo<TKey>
    {
        public Type ViewModelType { get; set; }

        public TKey ViewModelKey { get; set; }

        public ViewModelPluginInfo()
        {

        }

        public ViewModelPluginInfo
        (
            Type viewModelType,
            TKey viewModelKey)
        {
            ViewModelType = viewModelType;
            ViewModelKey = viewModelKey;
        }
    }
}
