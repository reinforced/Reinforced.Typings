using System;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
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

        public static ClassExportBuilder WithCodeGenerator<T>(this ClassExportBuilder conf, T codeGeneratorInstance)
            where T : ITsCodeGenerator<Type>
        {
            conf.Attr.CodeGeneratorInstance = codeGeneratorInstance;
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

        /// <summary>
        /// Configures class to export constructor
        /// If constructor body is not specified, then default body will be generated.
        /// Default body is empty if there are no inheritance.
        /// If there is inheritance then RT will try to generate optimal super() call 
        /// that can be controlled by <see cref="TsBaseParamAttribute"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf"></param>
        /// <param name="exportConstructors">When true, constructor will be exported</param>
        /// <param name="constructorBody">Optional constructor body implementation</param>
        /// <returns></returns>
        public static T WithConstructor<T>(this T conf, RtRaw constructorBody = null, bool exportConstructors = true)
            where T : ClassExportBuilder
        {
            conf.Attr.AutoExportConstructors = exportConstructors;
            conf.Blueprint.ConstructorBody = constructorBody;
            return conf;
        }
    }
}