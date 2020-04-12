using System.Reflection;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Generators;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class MemberExportExtensions
    {
        /// <summary>
        ///     Sets parameter default value.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="value">Default value for parameter</param>
        public static ParameterExportBuilder DefaultValue(this ParameterExportBuilder conf, object value)
        {
            conf.Attr.DefaultValue = value;
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static ParameterExportBuilder WithCodeGenerator<T>(this ParameterExportBuilder conf)
            where T : TsCodeGeneratorBase<ParameterInfo, RtArgument>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static ParameterExportBuilder WithCodeGenerator<T>(this ParameterExportBuilder conf, T codeGeneratorInstance)
            where T : TsCodeGeneratorBase<ParameterInfo, RtArgument>
        {
            conf.Attr.CodeGeneratorInstance = codeGeneratorInstance;
            return conf;
        }

    }
}