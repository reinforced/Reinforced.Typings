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
        public static PropertyExportBuilder WithCodeGenerator<T>(this PropertyExportBuilder conf)
            where T : ITsCodeGenerator<PropertyInfo>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        ///     Specifies code generator for member
        /// </summary>
        public static PropertyExportBuilder WithFieldCodeGenerator<T>(this PropertyExportBuilder conf)
            where T : ITsCodeGenerator<FieldInfo>
        {
            conf.Attr.CodeGeneratorType = typeof(T);
            return conf;
        }

        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        /// <param name="conf">Configurator</param>
        /// <param name="order">Order of member</param>
        /// <returns>Fluent</returns>
        public static PropertyExportBuilder Order(this PropertyExportBuilder conf, double order)
        {
            conf.Attr.Order = order;
            return conf;
        }

        /// <summary>
        ///     Forces property to be a nullable.
        ///     When set to true then property will be generated as [property]? : [type] with
        ///     forcibly added question mark denoting nullable field.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="force">Force nullable or not</param>
        public static PropertyExportBuilder ForceNullable(this PropertyExportBuilder conf, bool? force = true)
        {
            conf.Attr.NilForceNullable = force;
            return conf;
        }

        /// <summary>
        ///     Forces static property to be exported with constant initializer.
        ///     Works only on numeric/boolean/string/null properties
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="constant">Switches constant export behavior</param>
        public static PropertyExportBuilder Constant(this PropertyExportBuilder conf, bool constant = true)
        {
            conf.Attr.Constant = constant;
            return conf;
        }

        /// <summary>
        ///     Specifies initialization expression evaluator for property
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="evaluator">
        /// Initialization expression evaluator. Returns TypeScript code that will be used as initialization expression for 
        /// particular property
        /// </param>
        public static PropertyExportBuilder InitializeWith(this PropertyExportBuilder conf, Func<MemberInfo, TypeResolver, object, string> evaluator)
        {
            conf.Attr.InitializerEvaluator = evaluator;
            return conf;
        }
    }
}