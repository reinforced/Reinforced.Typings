using System;
using System.Collections.Generic;
using System.Reflection;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for class
    /// </summary>
    public class ClassExportBuilder : ClassOrInterfaceExportBuilder
    {
        internal ClassExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
            if (Blueprint.TypeAttribute == null)
                Blueprint.TypeAttribute = new TsClassAttribute
                {
                    AutoExportConstructors = false,
                    AutoExportFields = false,
                    AutoExportProperties = false,
                    AutoExportMethods = false
                };
        }

        internal TsClassAttribute Attr
        {
            get { return (TsClassAttribute)Blueprint.TypeAttribute; }
        }
    }

    /// <summary>
    /// Set of extensions for type export configuration
    /// </summary>
    public static partial class TypeConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript class
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static ClassExportBuilder<T> ExportAsClass<T>(this ConfigurationBuilder builder)
        {
            var bp = builder.GetCheckedBlueprint<TsClassAttribute>(typeof(T));

            var conf =
                builder.TypeExportBuilders.GetOrCreate(typeof(T), () => new ClassExportBuilder<T>(bp))
                    as ClassExportBuilder<T>;
            if (conf == null)
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(typeof(T), "class");
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
        public static void ExportAsClasses(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<ClassExportBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var untypedConf = builder.TypeExportBuilders.GetOrCreate(type, () =>
                {
                    var bp = builder.GetCheckedBlueprint<TsClassAttribute>(type);
                    if (!type._IsGenericTypeDefinition())
                    {
                        var t = typeof(ClassExportBuilder<>).MakeGenericType(type);
                        return (ClassExportBuilder)Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { bp }, null);
                    }

                    return new ClassExportBuilder(bp);
                });

                var conf = untypedConf as ClassExportBuilder;
                if (conf == null)
                {
                    ErrorMessages.RTE0017_FluentContradict.Throw(type, "class");
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