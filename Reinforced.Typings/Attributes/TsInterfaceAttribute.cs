using System;
// ReSharper disable VirtualMemberCallInConstructor

namespace Reinforced.Typings.Attributes
{
    /// <summary>
    ///     Exports specified class or interface as typescript interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class TsInterfaceAttribute : TsDeclarationAttributeBase, IAutoexportSwitchAttribute
    {
        /// <summary>
        ///     Constructs new instance of TsInterfaceAttribute
        /// </summary>
        public TsInterfaceAttribute()
        {
            AutoI = true;
            IncludeNamespace = true;
            AutoExportMethods = true;
            AutoExportProperties = true;
        }

        /// <summary>
        ///     Automatically appends I prefix if non-interfaces
        /// </summary>
        public virtual bool AutoI { get; set; }

        /// <summary>
        ///     Export all methods automatically or not.
        /// </summary>
        public virtual bool AutoExportMethods { get; set; }

        /// <summary>
        ///     Export all properties automatically or not.
        /// </summary>
        public virtual bool AutoExportProperties { get; set; }

        bool IAutoexportSwitchAttribute.AutoExportFields
        {
            get { return false; }
        }

        Type IAutoexportSwitchAttribute.DefaultMethodCodeGenerator
        {
            get { return null; }
        }

        bool IAutoexportSwitchAttribute.AutoExportConstructors
        {
            get { return false; }
        }
    }
}