// ReSharper disable CheckNamespace
namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Set of extensions for types exporting configuration
    /// </summary>
    public static partial class TypeExportExtensions
    {
        /// <summary>
        ///     Overrides name of exported type
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static T OverrideName<T>(this T conf, string name) where T : TypeExportBuilder
        {
            conf.Blueprint.TypeAttribute.Name = name;
            return conf;
        }

        /// <summary>
        ///     Configures exporter do not to export member to corresponding namespace
        /// </summary>
        public static T DontIncludeToNamespace<T>(this T conf, bool include = false)
            where T : TypeExportBuilder
        {
            conf.Blueprint.TypeAttribute.IncludeNamespace = include;
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
            conf.Blueprint.TypeAttribute.Namespace = nameSpace;
            return conf;
        }
    }
}