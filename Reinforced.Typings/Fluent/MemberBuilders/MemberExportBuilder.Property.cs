using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for property or field
    /// </summary>
    public class PropertyExportBuilder : MemberExportBuilder, ISupportsInferring<MemberInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal PropertyExportBuilder(TypeBlueprint containingTypeBlueprint, MemberInfo member) : base(containingTypeBlueprint, member)
        {
        }

        internal TsPropertyAttribute Attr
        {
            get { return (TsPropertyAttribute)_forMember; }
        }

        /// <summary>
        /// Gets property being configured
        /// </summary>
        public PropertyInfo Member
        {
            get { return (PropertyInfo) _member; }
        }

        /// <summary>
        /// Type inferers set instance
        /// </summary>
        public InlineTypeInferers<MemberInfo> TypeInferers
        {
            get { return Attr.TypeInferers; }
        }
    }
}