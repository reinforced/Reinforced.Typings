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
    }
}