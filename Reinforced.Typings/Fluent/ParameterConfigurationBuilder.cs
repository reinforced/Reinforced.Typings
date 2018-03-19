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
        private readonly TypeBlueprint _blueprint;
        internal ParameterConfigurationBuilder(ParameterInfo member, TypeBlueprint blueprint)
        {
            Member = member;
            _blueprint = blueprint;
            _blueprint.ForMember(Member, true);
        }

        private TsParameterAttribute AttributePrototype { get { return _blueprint.ForMember(Member, true); } }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.DecoratorsListFor(Member); }
        }

        TsParameterAttribute IAttributed<TsParameterAttribute>.AttributePrototype
        {
            get { return AttributePrototype; }
        }

        bool IIgnorable.Ignore
        {
            get { return _blueprint.IsIgnored(Member); }
            set
            {
                if (value) _blueprint.Ignored.Add(Member);
                else if (_blueprint.Ignored.Contains(Member)) _blueprint.Ignored.Remove(Member);
            }
        }

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