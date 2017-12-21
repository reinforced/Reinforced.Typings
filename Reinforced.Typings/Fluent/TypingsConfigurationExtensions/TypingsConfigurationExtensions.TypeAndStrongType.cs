using System;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static T Type<T>(this T conf, string typeName) where T : IAttributed<TsTypedAttributeBase>
        {
            conf.AttributePrototype.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static IAttributed<TsTypedAttributeBase> Type<T>(
            this IAttributed<TsTypedAttributeBase> conf)
        {
            conf.AttributePrototype.StrongType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="type">Type to override with</param>
        /// <returns></returns>
        public static T Type<T>(this T conf, Type type) where T : IAttributed<TsTypedAttributeBase>
        {
            conf.AttributePrototype.StrongType = type;
            return conf;
        }

        #region The same for methods

        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static T Returns<T>(this T conf, string typeName) where T : IAttributed<TsFunctionAttribute>
        {
            conf.AttributePrototype.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static IAttributed<TsTypedAttributeBase> Returns<T>(
            this IAttributed<TsFunctionAttribute> conf)
        {
            conf.AttributePrototype.StrongType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="type">Type to override with</param>
        /// <returns></returns>
        public static T Returns<T>(this T conf, Type type) where T : IAttributed<TsFunctionAttribute>
        {
            conf.AttributePrototype.StrongType = type;
            return conf;
        }

        #endregion
    }
}
