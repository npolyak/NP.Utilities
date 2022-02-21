using System;

namespace NP.Utilities.PluginUtils
{
    public class ViewModelPluginInfo
    {
        public Type ViewModelType { get; set; }

        public object ViewModelKey { get; set; }

        public ViewModelPluginInfo()
        {

        }

        public ViewModelPluginInfo
        (
            Type viewModelType,
            object viewModelKey)
        {
            ViewModelType = viewModelType;
            ViewModelKey = viewModelKey;
        }
    }
}
