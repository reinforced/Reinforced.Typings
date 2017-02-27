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

        private List<TsDecoratorAttribute> Decorators { get; set; }

        TsClassAttribute IExportConfiguration<TsClassAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
        }

        /// <inheritdoc />
        public override double MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }
    }
}