using System;

namespace Reinforced.Typings.Fluent
{
    public class GlobalConfigurationBuilder
    {
        internal GlobalParameters Parameters { get; private set; }

        public GlobalConfigurationBuilder(GlobalParameters parameters)
        {
            Parameters = parameters;
        }
    }

    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Configures global exporting parameters
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="config">Global configuration action</param>
        public static void Global(this ConfigurationBuilder builder, Action<GlobalConfigurationBuilder> config)
        {
            config(builder.GlobalBuilder);
        }

        /// <summary>
        /// Disables writing of "auto-generated warning" comment to each generated file.
        /// It meant the comment like "// This code was generated blah blah blah..."
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="dontWrite">Pass 'true' (default) to disable adding warning comment to target file. Pass 'false' to leave this label in place.</param>
        public static GlobalConfigurationBuilder DontWriteWarningComment(this GlobalConfigurationBuilder builder,
            bool dontWrite = true)
        {
            builder.Parameters.WriteWarningComment = !dontWrite;
            return builder;
        }

        /// <summary>
        ///     Specifies root namespace for hierarchical export.
        ///     Helps to avoid creating redundant directories when hierarchical export.
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="rootNamespace">Application root namespace</param>
        public static GlobalConfigurationBuilder RootNamespace(this GlobalConfigurationBuilder builder,
           string rootNamespace)
        {
            builder.Parameters.RootNamespace = rootNamespace;
            return builder;
        }

        /// <summary>
        /// Use camelCase for methods naming
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="cameCase">Pass 'true' to convert all MethodsNames to camelCase </param>
        public static GlobalConfigurationBuilder CamelCaseForMethods(this GlobalConfigurationBuilder builder,
            bool cameCase = true)
        {
            builder.Parameters.CamelCaseForMethods = cameCase;
            return builder;
        }

        /// <summary>
        /// Use camelCase for properties naming
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="cameCase">Pass 'true' to convert all MethodsNames to camelCase </param>
        public static GlobalConfigurationBuilder CamelCaseForProperties(this GlobalConfigurationBuilder builder,
            bool cameCase = true)
        {
            builder.Parameters.CamelCaseForProperties = cameCase;
            return builder;
        }

        /// <summary>
        /// Enables documentation generator
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="generate">Pass 'true' to generate JSDOC for exported types from XMLDOC</param>
        public static GlobalConfigurationBuilder GenerateDocumentation(this GlobalConfigurationBuilder builder,
            bool generate = true)
        {
            builder.Parameters.GenerateDocumentation = generate;
            return builder;
        }

        /// <summary>
        /// Enables adaptation for TS modules system (--modules tsc.exe option)
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="useModules">True will enable usage of modules system and exports/imports</param>
        /// <param name="discardNamespaces">True will automatically ignore namespaces while exporting types</param>
        public static GlobalConfigurationBuilder UseModules(this GlobalConfigurationBuilder builder,
            bool useModules = true, bool discardNamespaces = true)
        {
            builder.Parameters.UseModules = useModules;
            builder.Parameters.DiscardNamespacesWhenUsingModules = discardNamespaces;
            return builder;
        }

        /// <summary>
        /// Enables strict null checks. Particularry, makes all exported nullable value-types of type "type | null"
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="strict">Pass 'true' reveal all nullable types to "type | null" </param>
        public static GlobalConfigurationBuilder StrictNullChecks(this GlobalConfigurationBuilder builder,
            bool strict = true)
        {
            builder.Parameters.StrictNullChecks = strict;
            return builder;
        }
    }
}
