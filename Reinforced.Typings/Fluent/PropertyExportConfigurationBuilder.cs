using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Fluent configuration builder for exported properties
    /// </summary>
    public class PropertyExportConfigurationBuilder : IMemberExportConfiguration<TsPropertyAttribute, MemberInfo>, IIgnorable,
        IDecoratorsAggregator, IOrderableMember, ISupportsInferring<MemberInfo>
    {
        private readonly TypeBlueprint _blueprint;
        internal PropertyExportConfigurationBuilder(MemberInfo member, TypeBlueprint blueprint)
        {
            Member = member;
            _blueprint = blueprint;
            _blueprint.ForMember(Member, true);
        }

        List<TsDecoratorAttribute> IDecoratorsAggregator.Decorators
        {
            get { return _blueprint.DecoratorsListFor(Member); }
        }

        /// <summary>
        /// Property attribute prototype
        /// </summary>
        public TsPropertyAttribute AttributePrototype
        {
            get { return _blueprint.ForMember<TsPropertyAttribute>(Member, true); }
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
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<MemberInfo> TypeInferers { get { return AttributePrototype.TypeInferers; } }
    }
}