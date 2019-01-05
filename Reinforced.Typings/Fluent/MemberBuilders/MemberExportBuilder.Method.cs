using System.Reflection;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for method
    /// </summary>
    public class MethodExportBuilder : MemberExportBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        internal MethodExportBuilder(TypeBlueprint containingTypeBlueprint, MemberInfo member) : base(containingTypeBlueprint, member)
        {
        }

        internal TsFunctionAttribute Attr
        {
            get { return (TsFunctionAttribute)_forMember; }
        }
        
    }
}