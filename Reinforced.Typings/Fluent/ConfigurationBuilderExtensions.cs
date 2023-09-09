using System;
using System.IO;
using System.Reflection;
using Reinforced.Typings.Ast.Dependency;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Set of extensions for configuration builder
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds global reference to another typescript library
        /// </summary>
        /// <param name="conf">Table configurator</param>
        /// <param name="path">Full path to .d.ts or .ts file</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder AddReference(this ConfigurationBuilder conf, string path)
        {
            conf.Context.Project.References.Add(new RtReference { Path = path });
            return conf;
        }

        /// <summary>
        ///     Adds import directive to file containing typing for current type
        ///     This method is only used while splitting generated types to different files
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="target">
        /// What we are importing from module.
        /// Everything that is placed after "import" keyword and before "from" or "= require(..)"
        /// Examples: 
        /// - "import * as shape from './Shapes'" -> "* as shape" is target <br/>
        /// - "import { Foo } from 'Bar'" -> "{ Foo }" is target <br/>
        /// - "import { Bar2 as bar } from 'Baz'" -> "{ Bar2 as bar }" is target <br/>
        /// If ImportTarget is null then side-effect import will be generated. 
        /// </param>
        /// <param name="from">
        /// Import source is everything that follows after "from" keyword. 
        /// Please not the you do not have to specify quotes here! Quotes will be added automatically
        /// </param>
        /// <param name="isRequire">When true, import will be generated as "import ImportTarget = require('ImportSource')"</param>
        /// <param name="useDoubleQuotes">When true, import will use double quotes instead of single quotes"</param>
        public static ConfigurationBuilder AddImport(this ConfigurationBuilder conf, string target, string from,
            bool isRequire = false, bool useDoubleQuotes = false)
        {
            conf.Imports.Add(new RtImport()
            { Target = target, From = from, IsRequire = isRequire, UseDoubleQuotes = useDoubleQuotes });
            return conf;
        }

        /// <summary>
        /// Defines global type substitution. Substituted type will be strictly replaced with substitution during export
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="substitute">Type to substitute</param>
        /// <param name="substitution">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder Substitute(this ConfigurationBuilder builder, Type substitute,
            RtTypeName substitution)
        {
            builder.GlobalSubstitutions[substitute] = substitution;
            return builder;
        }

        /// <summary>
        /// Defines global generic type substitution. Substituted type will be strictly replaced with substitution during export
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="genericType">Type to substitute</param>
        /// <param name="substitutionFn">Substitution for type</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder SubstituteGeneric(this ConfigurationBuilder builder, Type genericType,
            Func<Type, TypeResolver, RtTypeName> substitutionFn)
        {
            if (!genericType._IsGenericTypeDefinition())
            {
                if (!genericType._IsGenericType())
                {
                    throw new Exception(string.Format(
                        "Type {0} does not appear to be generic type definition. Use MyType<> to define substitution",
                        genericType.FullName));
                }

                genericType = genericType.GetGenericTypeDefinition();
            }
            builder.GenericSubstitutions[genericType] = substitutionFn;
            return builder;
        }


        /// <summary>
        ///     Tries to find documentation .xml file for specified assembly and take it in account when generating documentaion
        /// </summary>
        /// <param name="conf">Table configurator</param>
        /// <param name="assmbly">Assembly which documentation should be included</param>
        /// <param name="documentationFileName">Override XMLDOC file name if differs (please include .xml extension)</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder TryLookupDocumentationForAssembly(this ConfigurationBuilder conf,
            Assembly assmbly, string documentationFileName = null)
        {
            if (!string.IsNullOrEmpty(documentationFileName)
                && Path.IsPathRooted(documentationFileName))
            {
                conf.AdditionalDocumentationPathes.Add(documentationFileName);
                return conf;
            }

            var locationFilePath = Path.Combine(
                string.IsNullOrEmpty(assmbly.Location) ? string.Empty : Path.GetDirectoryName(assmbly.Location),
                string.IsNullOrEmpty(documentationFileName)
                    ? Path.GetFileNameWithoutExtension(assmbly.Location) + ".xml"
                    : documentationFileName);

            var codebaseFilePath = Path.Combine(
                Path.GetDirectoryName(assmbly.GetCodeBase()),
                string.IsNullOrEmpty(documentationFileName)
                    ? Path.GetFileNameWithoutExtension(assmbly.CodeBase) + ".xml"
                    : documentationFileName);
            if (File.Exists(locationFilePath)) conf.AdditionalDocumentationPathes.Add(locationFilePath);
            else if (File.Exists(codebaseFilePath)) conf.AdditionalDocumentationPathes.Add(codebaseFilePath);
            return conf;
        }

        private static string GetCodeBase(this Assembly asmbly)
        {
            if (string.IsNullOrEmpty(asmbly.CodeBase)) return string.Empty;
            return asmbly.CodeBase.Replace("file:///", string.Empty);

        }
    }
}
