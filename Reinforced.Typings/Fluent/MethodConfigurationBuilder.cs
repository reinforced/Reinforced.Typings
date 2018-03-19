using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported methods
    /// </summary>
    public class MethodConfigurationBuilder : IMemberExportConfiguration<TsFunctionAttribute, MethodInfo>, IIgnorable,
        IDecoratorsAggregator, IOrderableMember, ISupportsInferring<MethodInfo>
    {
        private readonly TypeBlueprint _blueprint;
        internal MethodConfigurationBuilder(MethodInfo member, TypeBlueprint blueprint)
        {
            Member = member;
            _blueprint = blueprint;
            _blueprint.ForMember(Member, true);
        }

        private TsFunctionAttribute AttributePrototype
        {
            get { return _blueprint.ForMember(Member, true); }
        }


        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.DecoratorsListFor(Member); }
        }

        TsFunctionAttribute IAttributed<TsFunctionAttribute>.AttributePrototype
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

        double IOrderableMember.MemberOrder
        {
            get { return AttributePrototype.Order; }
            set { AttributePrototype.Order = value; }
        }

        /// <summary>
        /// Exporting member
        /// </summary>
        public MethodInfo Member { get; private set; }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<MethodInfo> TypeInferers { get { return AttributePrototype.TypeInferers; } }
    }
}