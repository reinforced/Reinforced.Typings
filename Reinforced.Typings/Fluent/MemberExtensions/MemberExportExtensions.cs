using System;
using Reinforced.Typings.Attributes;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Extensions for members export configuration
    /// </summary>
    public static partial class MemberExportExtensions
    {
        /// <summary>
        ///     Forces member name to be camelCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T CamelCase<T>(this T conf) where T : MemberExportBuilder
        {
            conf._forMember.ShouldBeCamelCased = true;
            return conf;
        }

        /// <summary>
        ///     Forces member name to be PascalCase
        /// </summary>
        /// <param name="conf">Configuration</param>
        public static T PascalCase<T>(this T conf) where T : MemberExportBuilder
        {
            conf._forMember.ShouldBePascalCased = true;
            return conf;
        }

        /// <summary>
        /// Adds decorator to member
        /// </summary>
        /// <param name="conf">Member configurator</param>
        /// <param name="decorator">Decorator to add (everything that must follow after "@")</param>
        /// <param name="order">Order of appearence</param>
        /// <returns>Fluent</returns>
        public static MemberExportBuilder Decorator(this MemberExportBuilder conf, string decorator, double order = 0)
        {
            conf.Decorators.Add(new TsDecoratorAttribute(decorator, order));
            return conf;
        }

        /// <summary>
        ///     Overrides name of exported type
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="name">Custom name to be used</param>
        public static MemberExportBuilder OverrideName(this MemberExportBuilder conf, string name)
        {
            conf._forMember.Name = name;
            return conf;
        }

        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static MemberExportBuilder Type(this MemberExportBuilder conf, string typeName)
        {
            conf._forMember.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static MemberExportBuilder Type<T>(this MemberExportBuilder conf)
        {
            conf._forMember.StrongType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="type">Type to override with</param>
        /// <returns></returns>
        public static MemberExportBuilder Type(this MemberExportBuilder conf, Type type)
        {
            conf._forMember.StrongType = type;
            return conf;
        }

        /// <summary>
        ///     Ignores specified members during exporting
        /// </summary>
        public static MemberExportBuilder Ignore(this MemberExportBuilder conf)
        {
            conf.IsIgnored = true;
            return conf;
        }
    }
}