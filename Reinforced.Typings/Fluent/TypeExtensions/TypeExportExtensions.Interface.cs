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
        public static InterfaceExportBuilder WithCodeGenerator<T>(this InterfaceExportBuilder conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }


        /// <summary>
        ///     Forces exporter to add I letter as interface prefix.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="auto">Add I automatically or not</param>
        public static InterfaceExportBuilder AutoI(this InterfaceExportBuilder conf, bool auto = true)
        {
            conf.Attr.AutoI = auto;
            return conf;
        }

    }
}