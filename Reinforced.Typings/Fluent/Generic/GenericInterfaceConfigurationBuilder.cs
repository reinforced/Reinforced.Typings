using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    class GenericInterfaceConfigurationBuilder : GenericConfigurationBuilderBase, IInterfaceConfigurationBuilder
    {
        public GenericInterfaceConfigurationBuilder(TypeBlueprint t, ExportContext context) : base(t, context)
        {
            if (_blueprint.TypeAttribute == null)
                _blueprint.TypeAttribute = new TsInterfaceAttribute
                {
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
        }

        public TsInterfaceAttribute AttributePrototype
        {
            get
            {
                return _blueprint.Attr<TsInterfaceAttribute>();
            }
        }

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