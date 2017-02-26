using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Parameter configuration builder
    /// </summary>
    public class ParameterConfigurationBuilder : IExportConfiguration<TsParameterAttribute>, IIgnorable, IDecoratorsAggregator
    {
        internal ParameterConfigurationBuilder()
        {
            AttributePrototype = new TsParameterAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsParameterAttribute AttributePrototype { get; set; }

        TsParameterAttribute IExportConfiguration<TsParameterAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }
        public List<TsDecoratorAttribute> Decorators { get; }
    }
}