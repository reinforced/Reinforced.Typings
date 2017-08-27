using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Parameter configuration builder
    /// </summary>
    public class ParameterConfigurationBuilder : IMemberExportConfiguration<TsParameterAttribute,ParameterInfo>, IIgnorable,
        IDecoratorsAggregator, ISupportsInferring<ParameterInfo>
    {
        internal ParameterConfigurationBuilder(ParameterInfo member)
        {
            Member = member;
            AttributePrototype = new TsParameterAttribute();
            Decorators = new List<TsDecoratorAttribute>();
        }

        private TsParameterAttribute AttributePrototype { get; set; }
        private List<TsDecoratorAttribute> Decorators { get; set; }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return Decorators; }
        }

        TsParameterAttribute IAttributed<TsParameterAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore { get; set; }

        /// <summary>
        /// Exporting member
        /// </summary>
        public ParameterInfo Member { get; private set; }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<ParameterInfo> TypeInferers { get { return AttributePrototype.TypeInferers; } }
    }
}