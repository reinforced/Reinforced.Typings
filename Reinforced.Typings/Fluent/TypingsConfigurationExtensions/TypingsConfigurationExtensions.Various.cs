using System;
using System.IO;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Fluent.Interfaces;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        ///     Forces property to be a nullable.
        ///     When set to true then property will be generated as [property]? : [type] with
        ///     forcibly added question mark denoting nullable field.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="force">Force nullable or not</param>
        public static T ForceNullable<T>(this T conf, bool? force = true)
            where T : IAttributed<TsPropertyAttribute>
        {
            conf.AttributePrototype.NilForceNullable = force;
            return conf;
        }

        /// <summary>
        ///     Forces static property to be exported with constant initializer.
        ///     Works only on numeric/boolean/string/null properties
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="constant">Switches constant export behavior</param>
        public static T Constant<T>(this T conf, bool constant = true)
            where T : IAttributed<TsPropertyAttribute>
        {
            conf.AttributePrototype.Constant = constant;
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
        public static T InitializeWith<T>(this T conf, Func<MemberInfo, TypeResolver, object, string> evaluator)
            where T : IAttributed<TsPropertyAttribute>
        {
            conf.AttributePrototype.InitializerEvaluator = evaluator;
            return conf;
        }

        /// <summary>
        ///     Sets parameter default value.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="value">Default value for parameter</param>
        public static T DefaultValue<T>(this T conf, object value) where T : IAttributed<TsParameterAttribute>
        {
            conf.AttributePrototype.DefaultValue = value;
            return conf;
        }

        /// <summary>
        ///     Forces exporter to add I letter as interface prefix.
        /// </summary>
        /// <param name="conf">Configuration</param>
        /// <param name="auto">Add I automatically or not</param>
        public static T AutoI<T>(this T conf, bool auto = true) where T : IAttributed<TsInterfaceAttribute>
        {
            conf.AttributePrototype.AutoI = auto;
            return conf;
        }



        /// <summary>
        ///     Tries to find documentation .xml file for specified assembly and take it in account when generating documentaion
        /// </summary>
        /// <param name="conf">Table configurator</param>
        /// <param name="assmbly">Assembly which documentation should be included</param>
        /// <param name="documentationFileName">Override XMLDOC file name if differs (please include .xml extension)</param>
        /// <returns>Fluent</returns>
        public static ConfigurationBuilder TryLookupDocumentationForAssembly(this ConfigurationBuilder conf,
            Assembly assmbly, string documentationFileName = null)
        {
            if (!string.IsNullOrEmpty(documentationFileName)
                && Path.IsPathRooted(documentationFileName))
            {
                conf.AdditionalDocumentationPathes.Add(documentationFileName);
                return conf;
            }

            var assemblyDir = Path.GetDirectoryName(assmbly.Location);
            var file = string.IsNullOrEmpty(documentationFileName)
                ? Path.GetFileNameWithoutExtension(assmbly.Location) + ".xml"
                : documentationFileName;
            var filePath = Path.Combine(assemblyDir, file);
            if (File.Exists(filePath))
            {
                conf.AdditionalDocumentationPathes.Add(filePath);
            }
            return conf;
        }

        /// <summary>
        /// Sets order this membter will be written to output file in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Configurator</param>
        /// <param name="order">Order of member</param>
        /// <returns>Fluent</returns>
        public static T Order<T>(this T conf, double order) where T : IOrderableMember
        {
            conf.MemberOrder = order;
            return conf;
        }



        /// <summary>
        /// Sets function body (works in case of class export) that will be converted to RtRaw and inserted as code block
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="functionCode">Function code</param>
        /// <returns></returns>
        public static T Implement<T>(this T builder, string functionCode)
            where T : IAttributed<TsFunctionAttribute>
        {
            builder.AttributePrototype.Implementation = functionCode;
            return builder;
        }

        /// <summary>
        /// Adds decorator to member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Member configurator</param>
        /// <param name="decorator">Decorator to add (everything that must follow after "@")</param>
        /// <param name="order">Order of appearence</param>
        /// <returns>Fluent</returns>
        public static T Decorator<T>(this T conf, string decorator, double order = 0) where T : IDecoratorsAggregator
        {
            conf.Decorators.Add(new TsDecoratorAttribute(decorator, order));
            return conf;
        }
    }
}
