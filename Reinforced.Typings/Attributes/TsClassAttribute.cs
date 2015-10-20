using System;

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    /// This attribute will export member as typescript class definition
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TsClassAttribute : TsDeclarationAttributeBase, IAutoexportSwitchAttribute
    {
        /// <summary>
        /// Export all methods automatically or not. 
        /// </summary>
        public virtual bool AutoExportMethods { get; set; }

        /// <summary>
        /// Export all properties automatically or not. 
        /// </summary>
        public virtual bool AutoExportProperties { get; set; }

        /// <summary>
        /// Export all fields automatically or not. 
        /// </summary>
        public virtual bool AutoExportFields { get; set; }

        /// <summary>
        /// Reference to code geenrator which will be applied to every method
        /// </summary>
        public virtual Type DefaultMethodCodeGenerator { get; set; }

        /// <summary>
        /// When true, code for all constructors will be automatically generated
        /// </summary>
        public bool AutoExportConstructors { get; set; }

        /// <summary>
        /// Constructs new instance of TsClassAttribute
        /// </summary>
        public TsClassAttribute()
        {
            AutoExportProperties = true;
            AutoExportMethods = true;
            IncludeNamespace = true;
            AutoExportConstructors = false;
        }
    }
}
