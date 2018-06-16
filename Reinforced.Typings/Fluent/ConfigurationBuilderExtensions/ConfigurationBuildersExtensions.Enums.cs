using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent.Interfaces;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class ConfigurationBuildersExtensions
    {
        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript enumeration
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static EnumConfigurationBuilder<T> ExportAsEnum<T>(this ConfigurationBuilder builder)
            where T : struct
        {
            return
                (EnumConfigurationBuilder<T>)
                builder.EnumConfigurationBuilders.GetOrCreate(typeof(T), () => new EnumConfigurationBuilder<T>(builder.Context));
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript enumerations
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<IEnumConfigurationBuidler> configuration = null)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = builder.EnumConfigurationBuilders.GetOrCreate(type, () =>
                {
                    var t = typeof(EnumConfigurationBuilder<>).MakeGenericType(tp);
                    return (IEnumConfigurationBuidler)Activator.CreateInstance(t, new[] { builder.Context });
                });
                if (configuration != null)
                {
                    try
                    {
                        configuration(conf);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "enum", type.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Turns enum to constant enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Enum configurator</param>
        /// <param name="isConst">When true, "const enum" will be generated. Regular enum otherwise</param>
        /// <returns>Fluent</returns>
        public static T Const<T>(this T conf, bool isConst = true) where T : IEnumConfigurationBuidler
        {
            conf.AttributePrototype.IsConst = isConst;
            return conf;
        }

        /// <summary>
        /// Makes enum to use string initializer for its values (TypeScript 2.4)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf">Enum configurator</param>
        /// <param name="useString">When true, enum values will be exported with string initializers</param>
        /// <returns>Fluent</returns>
        public static T UseString<T>(this T conf, bool useString = true) where T : IEnumConfigurationBuidler
        {
            conf.AttributePrototype.UseString = useString;
            return conf;
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="conf">Configuration builder</param>
        /// <param name="value">Enum value</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value<T>(this EnumConfigurationBuilder<T> conf, T value)
            where T : struct
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof(T)._GetField(n);
            var c = new EnumValueExportConfiguration(field, conf._blueprint);
            return c;
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <param name="conf">Configuration builder</param>
        /// <param name="propertyName">String enum property name</param>
        /// <returns>Configuration builder</returns>
        public static EnumValueExportConfiguration Value(this IEnumConfigurationBuidler conf, string propertyName)
        {
            var field = conf.EnumType._GetField(propertyName);
            var c = new EnumValueExportConfiguration(field, conf.Context.Project.Blueprint(conf.EnumType));
            return c;
        }

        /// <summary>
        ///     Configures export of particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="conf">Configuration builder</param>
        /// <param name="value">Enum value</param>
        /// <param name="valueConf">Enum value export configuration</param>
        /// <returns>Configuration builder</returns>
        public static EnumConfigurationBuilder<T> Value<T>(this EnumConfigurationBuilder<T> conf, T value, Action<EnumValueExportConfiguration> valueConf)
            where T : struct
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof(T)._GetField(n);
            var c = new EnumValueExportConfiguration(field, conf._blueprint);
            valueConf(c);
            return conf;
        }

        /// <summary>
        ///     Configures export of particular enumeration value
        /// </summary>
        /// <param name="conf">Configuration builder</param>
        /// <param name="propertyName">String enum property name</param>
        /// <param name="valueConf">Enum value export configuration</param>
        /// <returns>Configuration builder</returns>
        public static T Value<T>(this T conf, string propertyName, Action<EnumValueExportConfiguration> valueConf)
        where T: IEnumConfigurationBuidler
        {
            var field = conf.EnumType._GetField(propertyName);
            var c = new EnumValueExportConfiguration(field, conf.Context.Project.Blueprint(conf.EnumType));
            valueConf(c);
            return conf;
        }


        /// <summary>
        /// Overrides enum value's string initializer. Please escape quotes manually.
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static EnumValueExportConfiguration Initializer(this EnumValueExportConfiguration conf,
            string initializer)
        {
            var at = (IAttributed<TsValueAttribute>) conf;
            at.AttributePrototype.Initializer = initializer;
            return conf;
        }

    }
}
