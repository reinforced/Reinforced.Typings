using Reinforced.Typings.Ast.Dependency;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypeExportExtensions
    {
        /// <summary>
        ///  Overrides name of third-party type. Use full-qualified name.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fullQualifiedName">Full-qualified name of third-party type</param>
        /// <returns></returns>
        public static T WithName<T>(this T builder, string fullQualifiedName)
            where T : ThirdPartyExportBuilder
        {
            builder.Blueprint.ThirdParty.Name = fullQualifiedName;
            return builder;
        }

        /// <summary>
        /// Specifies set of references that third-party type will add to each file it is being used in
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="references">Set of references</param>
        /// <returns>Fluent</returns>
        public static T References<T>(this T builder, params RtReference[] references)
            where T : ThirdPartyExportBuilder
        {
            if (references != null) builder.Blueprint.ThirdPartyReferences.AddRange(references);
            return builder;
        }

        /// <summary>
        /// Specifies set of imports that third-party type will add to each file it is being used in
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="imports">Set of imports</param>
        /// <returns>Fluent</returns>
        public static T Imports<T>(this T builder, params RtImport[] imports)
            where T : ThirdPartyExportBuilder
        {
            if (imports != null) builder.Blueprint.ThirdPartyImports.AddRange(imports);
            return builder;
        }
    }
}