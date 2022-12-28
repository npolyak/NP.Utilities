using System;

namespace NP.Utilities.PluginUtils
{
    public class VisualPluginInfo<TKey> : ViewModelPluginInfo<TKey>
    {
        public string ViewDataTemplateResourcePath { get; set; }
        public object ViewDataTemplateResourceKey { get; set; }

        public VisualPluginInfo()
        {

        }

        public VisualPluginInfo
        (
            Type viewModelType, 
            TKey viewModelKey,
            string viewDatatemplateResourcePath,
            object viewDataTemplateResourceKey) 
            : 
            base(viewModelType, viewModelKey)
        {
            ViewDataTemplateResourcePath = viewDatatemplateResourcePath;
            ViewDataTemplateResourceKey = viewDataTemplateResourceKey;
        }
    }
}
