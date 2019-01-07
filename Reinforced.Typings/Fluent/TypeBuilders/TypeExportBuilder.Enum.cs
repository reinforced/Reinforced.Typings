using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for enum
    /// </summary>
    public class EnumExportBuilder : TypeExportBuilder
    {
        internal EnumExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
            if (Blueprint.TypeAttribute == null) Blueprint.TypeAttribute = new TsEnumAttribute();
        }

        internal TsEnumAttribute Attr
        {
            get { return (TsEnumAttribute)Blueprint.TypeAttribute; }
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <param name="propertyName">String enum property name</param>
        /// <returns>Configuration builder</returns>
        public EnumValueExportBuilder Value(string propertyName)
        {
            var field = Blueprint.Type._GetField(propertyName);
            var c = new EnumValueExportBuilder(Blueprint, field);
            return c;
        }



    }

    /// <summary>
    /// Fluent export configuration builder for enum (generic)
    /// </summary>
    public class EnumExportBuilder<T> : EnumExportBuilder where T : struct
    {
        internal EnumExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
        }

        /// <summary>
        ///     Retrieves configuration builder for particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="value">Enum value</param>
        /// <returns>Configuration builder</returns>
        public EnumValueExportBuilder Value(T value)
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof(T)._GetField(n);
            var c = new EnumValueExportBuilder(Blueprint, field);
            return c;
        }

        /// <summary>
        ///     Configures export of particular enumeration value
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="value">Enum value</param>
        /// <param name="valueConf">Enum value export configuration</param>
        /// <returns>Configuration builder</returns>
        public EnumExportBuilder<T> Value(T value, Action<EnumValueExportBuilder> valueConf)
        {
            var n = Enum.GetName(typeof(T), value);
            var field = typeof(T)._GetField(n);
            var c = new EnumValueExportBuilder(Blueprint, field);
            valueConf(c);
            return this;
        }
    }

    public static partial class TypeConfigurationBuilderExtensions
    {

        /// <summary>
        ///     Configures export of particular enumeration value
        /// </summary>
        /// <param name="conf">Configuration builder</param>
        /// <param name="propertyName">String enum property name</param>
        /// <param name="valueConf">Enum value export configuration</param>
        /// <returns>Configuration builder</returns>
        public static T Value<T>(this T conf, string propertyName, Action<EnumValueExportBuilder> valueConf)
            where T : EnumExportBuilder
        {
            var ve = conf.Value(propertyName);
            valueConf(ve);
            return conf;
        }

        /// <summary>
        /// Overrides enum value's string initializer. Please escape quotes manually.
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static EnumValueExportBuilder Initializer(this EnumValueExportBuilder conf, string initializer)
        {
            conf.Attr.Initializer = initializer;
            return conf;
        }



        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript enumeration
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static EnumExportBuilder<T> ExportAsEnum<T>(this ConfigurationBuilder builder)
            where T : struct
        {
            var bp = builder.GetCheckedBlueprint<TsEnumAttribute>(typeof(T));
            var conf =
                builder.TypeExportBuilders.GetOrCreate(typeof(T), () => new EnumExportBuilder<T>(bp))
                    as EnumExportBuilder<T>;
            if (conf == null)
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(typeof(T), "enum");
            }

            return conf;
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript enumerations
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsEnums(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<EnumExportBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var untypedConf = builder.TypeExportBuilders.GetOrCreate(type, () =>
                {
                    var bp = builder.GetCheckedBlueprint<TsEnumAttribute>(type);
                    var t = typeof(EnumExportBuilder<>).MakeGenericType(type);
                    return (EnumExportBuilder)Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { bp }, null);
                });

                var conf = untypedConf as EnumExportBuilder;
                if (conf == null)
                {
                    ErrorMessages.RTE0017_FluentContradict.Throw(type, "enum");
                }

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
    }
}