using System;

namespace NP.Utilities.PluginUtils
{
    public class VisualPluginInfo : ViewModelPluginInfo
    {
        public string ViewDataTemplateResourcePath { get; set; }
        public object ViewDataTemplateResourceKey { get; set; }

        public VisualPluginInfo()
        {

        }

        public VisualPluginInfo
        (
            Type viewModelType, 
            object viewModelKey,
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
