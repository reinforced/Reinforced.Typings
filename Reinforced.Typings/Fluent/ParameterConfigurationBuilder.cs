using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Parameter configuration builder
    /// </summary>
    public class ParameterConfigurationBuilder : IExportConfiguration<TsParameterAttribute>, IIgnorable,
        IDecoratorsAggregator
    {
        internal ParameterConfigurationBuilder()
        {
            AttributePrototype = new TsParameterAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsParameterAttribute AttributePrototype { get; set; }
        private List<TsDecoratorAttribute> Decorators { get; set; }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
        }

        TsParameterAttribute IExportConfiguration<TsParameterAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
    }
}