using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for property or field
    /// </summary>
    public class PropertyExportBuilder : MemberExportBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal PropertyExportBuilder(TypeBlueprint containingTypeBlueprint, MemberInfo member) : base(containingTypeBlueprint, member)
        {
        }

        internal TsPropertyAttribute Attr
        {
            get { return (TsPropertyAttribute)_forMember; }
        }
    }
}