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
        /// Gets whether type configuration is flattern
        /// </summary>
        public override bool IsHierarchyFlattern { get { return AttributePrototype.FlatternHierarchy; } }

        /// <inheritdoc />
        public override Type FlatternLimiter { get { return AttributePrototype.FlatternLimiter; } }

        /// <inheritdoc />
        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}