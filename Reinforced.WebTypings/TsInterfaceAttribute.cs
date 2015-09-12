using System;

namespace Reinforced.WebTypings
{
    /// <summary>
    /// Exports specified class or interface as typescript interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class TsInterfaceAttribute : TsDeclarationAttributeBase, IAutoexportSwitch
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

        bool IAutoexportSwitch.AutoExportFields
        {
            get { return false; }
        }

        Type IAutoexportSwitch.DefaultMethodCodeGenerator
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
