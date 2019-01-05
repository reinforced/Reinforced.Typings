using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Configuration builder for interface
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public class InterfaceConfigurationBuilder<TInterface> : TypeConfigurationBuilder<TInterface>,
        IInterfaceConfigurationBuilder
    {
        internal InterfaceConfigurationBuilder(ExportContext context) : base(context)
        {
            if (_blueprint.TypeAttribute == null)
                _blueprint.TypeAttribute = new TsInterfaceAttribute
                {
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
        }
        
        /// <summary>
        /// Interface attribute prototype
        /// </summary>
        public TsInterfaceAttribute AttributePrototype
        {
            get { return _blueprint.Attr<TsInterfaceAttribute>(); }
        }

        /// <summary>
        /// Gets whether type configuration is flatten
        /// </summary>
        public override bool IsHierarchyFlatten { get { return AttributePrototype.FlattenHierarchy; } }

        /// <inheritdoc />
        public override Type FlattenLimiter { get { return AttributePrototype.FlattenLimiter; } }

        /// <inheritdoc />
        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}