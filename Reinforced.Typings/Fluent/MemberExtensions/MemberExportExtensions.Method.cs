using System;
using System.Reflection;
using Reinforced.Typings.Generators;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class MemberExportExtensions
    {
        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static MethodExportBuilder WithCodeGenerator<T>(this MethodExportBuilder conf)
            where T : ITsCodeGenerator<MethodInfo>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static MethodExportBuilder WithCodeGenerator<T>(this MethodExportBuilder conf, T codeGeneratorInstance)
            where T : ITsCodeGenerator<MethodInfo>
        {
            conf.Attr.CodeGeneratorInstance = codeGeneratorInstance;
            return conf;
        }

        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="order">Order of member</param>
        /// <returns>Fluent</returns>
        public static MethodExportBuilder Order(this MethodExportBuilder conf, double order)
        {
            conf.Attr.Order = order;
            return conf;
        }

        /// <summary>
        /// Sets function body (works in case of class export) that will be converted to RtRaw and inserted as code block
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="functionCode">Function code</param>
        /// <returns></returns>
        public static MethodExportBuilder Implement(this MethodExportBuilder builder, string functionCode)
        {
            builder.Attr.Implementation = functionCode;
            return builder;
        }

        /// <summary>
        ///     Overrides member type name on export with textual string.
        ///     Beware of using this setting because specified type may not present in your TypeScript code and
        ///     this will lead to TypeScript compilation errors.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="typeName">TS-friendly type name</param>
        /// <returns></returns>
        public static MethodExportBuilder Returns(this MethodExportBuilder conf, string typeName)
        {
            conf.Attr.Type = typeName;
            return conf;
        }

        /// <summary>
        ///     Overrides member type on export with strong type.
        ///     Feel free to use delegates here. It is very comfortable instead of regular TS functions syntax.
        ///     Actually this method does the same as .Type call. Just for your convinence
        /// </summary>
        /// <param name="conf">Configurator</param>
        public static MethodExportBuilder Returns<T>(this MethodExportBuilder conf)
        {
            conf.Attr.StrongType = typeof(T);
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
        public static MethodExportBuilder Returns(this MethodExportBuilder conf, Type type) 
        {
            conf.Attr.StrongType = type;
            return conf;
        }
    }
}