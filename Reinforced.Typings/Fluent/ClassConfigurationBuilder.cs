using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Export configuration builder for class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassConfigurationBuilder<T> : TypeConfigurationBuilder<T>, IClassConfigurationBuilder
    {
        public ClassConfigurationBuilder(ExportContext context) : base(context)
        {
            if (_blueprint.TypeAttribute == null)
            {
                _blueprint.TypeAttribute = new TsClassAttribute
                {
                    AutoExportConstructors = false,
                    AutoExportFields = false,
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
            }
        }

        /// <summary>
        /// Class TS attribute prototype
        /// </summary>
        public TsClassAttribute AttributePrototype
        {
            get { return _blueprint.Attr<TsClassAttribute>(); }
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.Decorators; }
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