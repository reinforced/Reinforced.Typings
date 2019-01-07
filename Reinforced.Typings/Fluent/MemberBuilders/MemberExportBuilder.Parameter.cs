using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for method parameter
    /// </summary>
    public class ParameterExportBuilder : MemberExportBuilder, ISupportsInferring<ParameterInfo>
    {
        private readonly ParameterInfo _parMember;

        internal ParameterExportBuilder(TypeBlueprint containingTypeBlueprint, ParameterInfo member)
            : base(containingTypeBlueprint, null)
        {
            _parMember = member;
            _forMember = containingTypeBlueprint.ForMember(member, true);
        }

        internal TsParameterAttribute Attr
        {
            get { return (TsParameterAttribute)_forMember; }
        }

        internal override bool IsIgnored
        {
            get { return _containingTypeBlueprint.IsIgnored(_parMember); }
            set
            {
                if (value) _containingTypeBlueprint.Ignored.Add(_parMember);
                else if (_containingTypeBlueprint.Ignored.Contains(_parMember)) _containingTypeBlueprint.Ignored.Remove(_parMember);
            }
        }

        internal override List<TsDecoratorAttribute> Decorators
        {
            get { return _containingTypeBlueprint.DecoratorsListFor(_parMember); }
        }

        /// <summary>
        /// Gets parameter being configured
        /// </summary>
        public ParameterInfo Member
        {
            get { return _parMember; }
        }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<ParameterInfo> TypeInferers
        {
            get { return Attr.TypeInferers; }
        }
    }
}