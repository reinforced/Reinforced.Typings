using System;
using System.Collections.Generic;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent.Generic;
using Reinforced.Typings.Fluent.Interfaces;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class ConfigurationBuildersExtensions
    {
        /// <summary>
        ///     Includes specified type to resulting typing exported as TypeScript class
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> ExportAsClass<T>(this ConfigurationBuilder builder)
        {
            return
                (ClassConfigurationBuilder<T>)
                builder.TypeConfigurationBuilders.GetOrCreate(typeof(T), () => new ClassConfigurationBuilder<T>());
        }

        /// <summary>
        ///     Includes specified types to resulting typing exported as TypeScript classes
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsClasses(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<IClassConfigurationBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                var tp = type;
                var conf = (IClassConfigurationBuilder)builder.TypeConfigurationBuilders.GetOrCreate(type, () =>
                {
                    Type t = null;
                    if (!tp._IsGenericType())
                    {
                        t = typeof(ClassConfigurationBuilder<>).MakeGenericType(tp);
                        return (IClassConfigurationBuilder)Activator.CreateInstance(t, true);
                    }
                    t = typeof(GenericClassConfigurationBuilder);
                    return (ITypeConfigurationBuilder)Activator.CreateInstance(t, tp);
                });
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
