using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for interface
    /// </summary>
    public class InterfaceExportBuilder : ClassOrInterfaceExportBuilder
    {
        internal InterfaceExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
            if (Blueprint.TypeAttribute == null)
                Blueprint.TypeAttribute = new TsInterfaceAttribute
                {
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
        }

        internal TsInterfaceAttribute Attr
        {
            get { return (TsInterfaceAttribute)Blueprint.TypeAttribute; }
        }
    }

    public static partial class TypeConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript class
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static InterfaceExportBuilder<T> ExportAsInterface<T>(this ConfigurationBuilder builder)
        {
            var bp = builder.GetCheckedBlueprint<TsInterfaceAttribute>(typeof(T));

            var conf =
                builder.TypeExportBuilders.GetOrCreate(typeof(T), () => new InterfaceExportBuilder<T>(bp))
                as InterfaceExportBuilder<T>;
            if (conf == null)
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(typeof(T), "interface");
            }

            return conf;

        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript classes
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsInterfaces(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<InterfaceExportBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var untypedConf = builder.TypeExportBuilders.GetOrCreate(type, () =>
                {
                    var bp = builder.GetCheckedBlueprint<TsInterfaceAttribute>(type);

                    if (!type._IsGenericTypeDefinition())
                    {
                        var t = typeof(InterfaceExportBuilder<>).MakeGenericType(type);
                        return (InterfaceExportBuilder)t.InstanceInternal(bp);
                    }

                    return new InterfaceExportBuilder(bp);
                });

                var conf = untypedConf as InterfaceExportBuilder;
                if (conf == null)
                {
                    ErrorMessages.RTE0017_FluentContradict.Throw(type, "interface");
                }

                if (configuration != null)
                {
                    try
                    {
                        configuration(conf);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.RTE0006_FluentSingleError.Throw(ex.Message, "type", type.FullName);
                    }
                }
            }
        }
    }
}