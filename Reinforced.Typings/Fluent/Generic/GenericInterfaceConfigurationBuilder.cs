using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    class GenericInterfaceConfigurationBuilder : GenericConfigurationBuilderBase, IInterfaceConfigurationBuilder
    {
        public GenericInterfaceConfigurationBuilder(Type t) : base(t)
        {
            AttributePrototype = new TsInterfaceAttribute
            {
                AutoExportProperties = false,
                AutoExportMethods = false
            };
        }

        public TsInterfaceAttribute AttributePrototype { get; private set; }

        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }

        /// <summary>
        /// Gets whether type configuration is flatten
        /// </summary>
        public override bool IsHierarchyFlatten { get { return AttributePrototype.FlattenHierarchy; } }

        /// <summary>
        /// Flatten limiter
        /// </summary>
        public override Type FlattenLimiter { get { return AttributePrototype.FlattenLimiter; } }
    }
}