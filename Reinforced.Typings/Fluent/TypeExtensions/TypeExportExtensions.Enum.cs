using System;
using Reinforced.Typings.Generators;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypeExportExtensions
    {
        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static EnumExportBuilder WithCodeGenerator<T>(this EnumExportBuilder conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        /// Turns enum to constant enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Enum configurator</param>
        /// <param name="isConst">When true, "const enum" will be generated. Regular enum otherwise</param>
        /// <returns>Fluent</returns>
        public static T Const<T>(this T conf, bool isConst = true) where T : EnumExportBuilder
        {
            conf.Attr.IsConst = isConst;
            return conf;
        }

        /// <summary>
        /// Makes enum to use string initializer for its values (TypeScript 2.4)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Enum configurator</param>
        /// <param name="useString">When true, enum values will be exported with string initializers</param>
        /// <returns>Fluent</returns>
        public static T UseString<T>(this T conf, bool useString = true) where T : EnumExportBuilder
        {
            conf.Attr.UseString = useString;
            return conf;
        }
    }
}