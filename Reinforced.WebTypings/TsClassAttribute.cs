using System;

namespace Reinforced.WebTypings
{
    /// <summary>
    /// This attribute will export member as typescript class definition
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TsClassAttribute : TsDeclarationAttributeBase, IAutoexportSwitch
    {
        /// <summary>
        /// Export all methods automatically or not. 
        /// </summary>
        public bool AutoExportMethods { get; set; }

        /// <summary>
        /// Export all properties automatically or not. 
        /// </summary>
        public bool AutoExportProperties { get; set; }

        /// <summary>
        /// Export all fields automatically or not. 
        /// </summary>
        public bool AutoExportFields { get; set; }

        /// <summary>
        /// Reference to code geenrator which will be applied to every method
        /// </summary>
        public Type DefaultMethodCodeGenerator { get; set; }

        public TsClassAttribute()
        {
            AutoExportProperties = true;
            AutoExportFields = true;
            AutoExportMethods = true;
            IncludeNamespace = true;
        }
    }
}
