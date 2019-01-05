using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for Type Member
    /// </summary>
    public class MemberExportBuilder
    {
        internal TypeBlueprint _containingTypeBlueprint;
        internal readonly MemberInfo _member;
        internal TsTypedMemberAttributeBase _forMember;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal MemberExportBuilder(TypeBlueprint containingTypeBlueprint, MemberInfo member)
        {
            _containingTypeBlueprint = containingTypeBlueprint;
            if (member != null)
            {
                _member = member;
                _forMember = containingTypeBlueprint.ForMember(member);
            }
        }

        internal virtual bool IsIgnored
        {
            get { return _containingTypeBlueprint.IsIgnored(_member); }
            set
            {
                if (value) _containingTypeBlueprint.Ignored.Add(_member);
                else if (_containingTypeBlueprint.Ignored.Contains(_member)) _containingTypeBlueprint.Ignored.Remove(_member);
            }
        }

        internal virtual List<TsDecoratorAttribute> Decorators
        {
            get { return _containingTypeBlueprint.DecoratorsListFor(_member); }
        }

    }
}
