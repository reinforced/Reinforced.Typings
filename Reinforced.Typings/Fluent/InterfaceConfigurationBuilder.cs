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
        internal InterfaceConfigurationBuilder()
        {
            AttributePrototype = new TsInterfaceAttribute
            {
                AutoExportProperties = false,
                AutoExportMethods = false
            };
        }

        private TsInterfaceAttribute AttributePrototype { get; set; }

        TsInterfaceAttribute IAttributed<TsInterfaceAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
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