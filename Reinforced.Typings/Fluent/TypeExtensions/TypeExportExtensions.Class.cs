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
        public static ClassExportBuilder WithCodeGenerator<T>(this ClassExportBuilder conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///   Configures class to be exported as abstract or not.
        ///   Pass null value to identify automatically
        /// </summary>
        public static T Abstract<T>(this T conf, bool? isAbstract = true)
            where T : ClassExportBuilder
        {
            conf.Attr.IsAbstract = isAbstract;

            return conf;
        }
    }
}