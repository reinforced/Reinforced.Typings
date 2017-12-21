using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Overrides name of specified member
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static T OverrideName<T>(this T conf, string name) where T : IAttributed<INameOverrideAttribute>
        {
            conf.AttributePrototype.Name = name;
            return conf;
        }

        /// <summary>
        ///     Forces member name to be camelCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T CamelCase<T>(this T conf) where T : IAttributed<ICamelCaseableAttribute>
        {
            conf.AttributePrototype.ShouldBeCamelCased = true;
            return conf;
        }

        /// <summary>
        ///     Forces member name to be PascalCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T PascalCase<T>(this T conf) where T : IAttributed<IPascalCasableAttribute>
        {
            conf.AttributePrototype.ShouldBePascalCased = true;
            return conf;
        }

        /// <summary>
        ///     Configures exporter dont to export member to corresponding namespace
        /// </summary>
        public static T DontIncludeToNamespace<T>(this T conf, bool include = false)
            where T : IAttributed<TsDeclarationAttributeBase>
        {
            conf.AttributePrototype.IncludeNamespace = include;
            return conf;
        }

        /// <summary>
        ///     Configures exporter to export type to specified namespace
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="nameSpace">Namespace name</param>
        public static T OverrideNamespace<T>(this T conf, string nameSpace)
            where T : IAttributed<TsDeclarationAttributeBase>
        {
            conf.AttributePrototype.Namespace = nameSpace;
            return conf;
        }
    }
}
