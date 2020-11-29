using System.Reflection;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for a field
    /// </summary>
    public class FieldExportBuilder : PropertyExportBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal FieldExportBuilder(TypeBlueprint containingTypeBlueprint, MemberInfo member) : base(containingTypeBlueprint, member)
        {
        }

        /// <summary>
        /// Gets property being configured
        /// </summary>
        public new FieldInfo Member
        {
            get { return (FieldInfo) _member; }
        }
    }
}