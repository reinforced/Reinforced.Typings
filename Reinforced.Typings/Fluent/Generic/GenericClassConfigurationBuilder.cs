using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent.Generic
{
    class GenericClassConfigurationBuilder : GenericConfigurationBuilderBase, IClassConfigurationBuilder
    {
        public GenericClassConfigurationBuilder(TypeBlueprint t, ExportContext context) : base(t, context)
        {
            if (_blueprint.TypeAttribute == null)
                _blueprint.TypeAttribute = new TsClassAttribute
                {
                    AutoExportConstructors = false,
                    AutoExportFields = false,
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
        }

        /// <summary>
        /// Class attribute prototype
        /// </summary>
        public TsClassAttribute AttributePrototype
        {
            get { return AttributePrototype; }
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.Decorators; }
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

        /// <inheritdoc />
        public override Type FlattenLimiter { get { return AttributePrototype.FlattenLimiter; } }
    }
}