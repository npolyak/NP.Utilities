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

    public class ViewModelPluginInfo : ViewModelPluginInfo<object?>
    {
        public ViewModelPluginInfo()
        {
            
        }

        public ViewModelPluginInfo
        (
            Type viewModelType,
            object? viewModelKey) : base(viewModelType, viewModelKey) 
        {
            
        }
    }

    public class ViewModelPluginInfoStr : ViewModelPluginInfo<string?>
    {
        public ViewModelPluginInfoStr()
        {

        }

        public ViewModelPluginInfoStr
        (
            Type viewModelType,
            string? viewModelKey) : base(viewModelType, viewModelKey)
        {

        }
    }

    public class ViewModelPluginInfoEnum : ViewModelPluginInfo<Enum>
    {
        public ViewModelPluginInfoEnum()
        {

        }

        public ViewModelPluginInfoEnum
        (
            Type viewModelType,
            Enum viewModelKey) : base(viewModelType, viewModelKey)
        {

        }
    }
}
