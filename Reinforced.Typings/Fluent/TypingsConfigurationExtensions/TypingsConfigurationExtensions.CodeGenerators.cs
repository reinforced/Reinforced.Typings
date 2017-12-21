using System;
using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;
using Reinforced.Typings.Generators;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsClassAttribute> WithCodeGenerator<T>(
            this IAttributed<TsClassAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsInterfaceAttribute> WithCodeGenerator<T>(
            this IAttributed<TsInterfaceAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsEnumAttribute> WithCodeGenerator<T>(
            this IAttributed<TsEnumAttribute> conf)
            where T : ITsCodeGenerator<Type>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsPropertyAttribute> WithCodeGenerator<T>(
            this IAttributed<TsPropertyAttribute> conf)
            where T : ITsCodeGenerator<MemberInfo>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsFunctionAttribute> WithCodeGenerator<T>(
            this IAttributed<TsFunctionAttribute> conf)
            where T : ITsCodeGenerator<MethodInfo>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static IAttributed<TsParameterAttribute> WithCodeGenerator<T>(
            this IAttributed<TsParameterAttribute> conf)
            where T : TsCodeGeneratorBase<ParameterInfo, RtArgument>
        {
            conf.AttributePrototype.CodeGeneratorType = typeof(T);
            return conf;
        }
    }
}
