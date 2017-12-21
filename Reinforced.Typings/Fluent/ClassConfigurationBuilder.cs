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
        internal ClassConfigurationBuilder()
        {
            AttributePrototype = new TsClassAttribute
            {
                AutoExportConstructors = false,
                AutoExportFields = false,
                AutoExportProperties = false,
                AutoExportMethods = false
            };
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsClassAttribute AttributePrototype { get; set; }

        TsClassAttribute IAttributed<TsClassAttribute>.AttributePrototype
        {
            get { return this.AttributePrototype; }
        }

        private List<TsDecoratorAttribute> Decorators { get; set; }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
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