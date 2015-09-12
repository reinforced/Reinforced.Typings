using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// Exports specified class or interface as typescript interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class TsInterfaceAttribute : TsDeclarationAttributeBase, IAutoexportSwitchAttribute
    {
        /// <summary>
        /// Automatically appends I prefix if non-interfaces
        /// </summary>
        public bool AutoI { get; set; }
        
        /// <summary>
        /// Export all methods automatically or not. 
        /// </summary>
        public bool AutoExportMethods { get; set; }

        /// <summary>
        /// Export all properties automatically or not. 
        /// </summary>
        public bool AutoExportProperties { get; set; }

        bool IAutoexportSwitchAttribute.AutoExportFields
        {
            get { return false; }
        }

        Type IAutoexportSwitchAttribute.DefaultMethodCodeGenerator
        {
            get { return null; }
        }

        public TsInterfaceAttribute()
        {
            AutoI = true;
            IncludeNamespace = true;
            AutoExportMethods = true;
            AutoExportProperties = true;
        }
    }
}
