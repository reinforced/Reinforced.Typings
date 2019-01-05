using System;
using Reinforced.Typings.Ast.TypeNames;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypeExportExtensions
    {
        /// <summary>
        /// Defines local type substitution that will work only when exporting current class. 
        /// Substituted type will be strictly replaced with substitution during export but this option will take effect only when 
        /// exporting currently configurable type
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="substitute">Type to substitute</param>
        /// <param name="substitution">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static T Substitute<T>(this T builder, Type substitute, RtTypeName substitution)
            where T : TypeExportBuilder
        {
            builder.Blueprint.Substitutions[substitute] = substitution;
            return builder;
        }

        /// <summary>
        /// Defines local generic type substitution that will work only when exporting current class. 
        /// Substituted type will be strictly replaced with substitution during export but this option will take effect only when 
        /// exporting currently configurable type
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="genericType">Type to substitute</param>
        /// <param name="substitutionFn">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static T SubstituteGeneric<T>(this T builder, Type genericType,
            Func<Type, TypeResolver, RtTypeName> substitutionFn)
            where T : TypeExportBuilder
        {
            if (!genericType._IsGenericTypeDefinition())
            {
                if (!genericType._IsGenericType())
                {
                    throw new Exception(string.Format(
                        "Type {0} does not appear to be generic type definition. Use typeof(MyType<>) to define generic substitution",
                        genericType.FullName));
                }

                genericType = genericType.GetGenericTypeDefinition();
            }

            builder.Blueprint.GenericSubstitutions[genericType] = substitutionFn;
            return builder;
        }
    }
}