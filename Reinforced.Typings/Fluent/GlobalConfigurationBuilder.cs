using System;
using Reinforced.Typings.ReferencesInspection;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Global configuration builder
    /// </summary>
    public class GlobalConfigurationBuilder
    {
        internal GlobalConfigurationBuilder(GlobalParameters parameters)
        {
            Parameters = parameters;
        }

        internal GlobalParameters Parameters { get; private set; }
    }

    /// <summary>
    /// Set of extensions for global configuration
    /// </summary>
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
        /// Changes indentation symbol (by default is \t). 
        /// This ability is made by @jonsa's request - boring perfectionist
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="symbol">New indentation symbol</param>
        public static GlobalConfigurationBuilder TabSymbol(this GlobalConfigurationBuilder builder,
            string symbol)
        {
            builder.Parameters.TabSymbol = symbol;
            return builder;
        }

        /// <summary>
        /// Changes line termination string. Default is <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="newLine">String that used as the line terminator.</param>
        public static GlobalConfigurationBuilder NewLine(this GlobalConfigurationBuilder builder,
            string newLine) 
        {
            builder.Parameters.NewLine = newLine;
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
        ///  If true, export will be performed in .d.ts manner (only typings, declare module etc).
        ///  Otherwise, export will be performed to regulat .ts file
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="typings">Enables or disables option</param>
        public static GlobalConfigurationBuilder ExportPureTypings(this GlobalConfigurationBuilder builder,
            bool typings = true)
        {
            builder.Parameters.ExportPureTypings = typings;
            return builder;
        }

        /// <summary>
        ///  Sets rype of <see cref="Reinforced.Typings.ReferencesInspection.ReferenceProcessorBase"/> to be used to
        /// refilter/reorder references and imports while exporting files
        /// </summary>
        /// <typeparam name="T">Type of references processor to be used</typeparam>
        /// <param name="builder">Conf builder</param>
        /// <param name="use">When false then disables usage of references processor</param>
        public static GlobalConfigurationBuilder WithReferencesProcessor<T>(this GlobalConfigurationBuilder builder, bool use = true)
        where T:ReferenceProcessorBase
        {
            builder.Parameters.ReferencesProcessorType = use ? typeof(T) : null;
            return builder;
        }

        /// <summary>
        ///  Enables or disables exporting members reordering (aphabetical, constructors-fields-properties-methods).
        ///  Warning! Enabling this option discards <see cref="MemberExportExtensions.Order(Reinforced.Typings.Fluent.MethodExportBuilder,double)"/> calls as well as "Order" member attributes property
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="reorder">True to reorder exported members alphabetically, false otherwise</param>
        public static GlobalConfigurationBuilder ReorderMembers(this GlobalConfigurationBuilder builder, bool reorder = true)
        {
            builder.Parameters.ReorderMembers = reorder;
            return builder;
        }

        /// <summary>
        ///  Tells RT to make all nullable value-type properties optional automatically
        /// </summary>
        /// <param name="builder">Conf builder</param>
        /// <param name="autoOptional">True to export make all nullable-typed properties optional</param>
        public static GlobalConfigurationBuilder AutoOptionalProperties(this GlobalConfigurationBuilder builder, bool autoOptional = true)
        {
            builder.Parameters.AutoOptionalProperties = autoOptional;
            return builder;
        }

        //{
        //    bool strict = true)
        //public static GlobalConfigurationBuilder StrictNullChecks(this GlobalConfigurationBuilder builder,
        ///// <param name="strict">Pass 'true' reveal all nullable types to "type | null" </param>
        ///// <param name="builder">Conf builder</param>
        ///// </summary>
        ///// Enables strict null checks. Particularry, makes all exported nullable value-types of type "type | null"

        ///// <summary>
        //    builder.Parameters.StrictNullChecks = strict;
        //    return builder;
        //}
    }
}