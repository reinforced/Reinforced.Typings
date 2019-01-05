using System;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Fluent.New
{
    public static class TypeExportExtensions
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
            builder._blueprint.Substitutions[substitute] = substitution;
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
            builder._blueprint.GenericSubstitutions[genericType] = substitutionFn;
            return builder;
        }

        /// <summary>
        ///     Overrides name of exported type
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static T OverrideName<T>(this T conf, string name) where T : TypeExportBuilder
        {
            conf._blueprint.TypeAttribute.Name = name;
            return conf;
        }

        /// <summary>
        ///     Configures exporter do not to export member to corresponding namespace
        /// </summary>
        public static T DontIncludeToNamespace<T>(this T conf, bool include = false)
            where T : TypeExportBuilder
        {
            conf._blueprint.TypeAttribute.IncludeNamespace = include;
            return conf;
        }

        /// <summary>
        ///     Configures exporter to export type to specified namespace
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="nameSpace">Namespace name</param>
        public static T OverrideNamespace<T>(this T conf, string nameSpace)
            where T : TypeExportBuilder
        {
            conf._blueprint.TypeAttribute.Namespace = nameSpace;
            return conf;
        }
    }
}