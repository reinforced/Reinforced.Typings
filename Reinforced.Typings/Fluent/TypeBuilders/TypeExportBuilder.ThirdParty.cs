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
    public class ThirdPartyExportBuilder
    {
        internal TypeBlueprint Blueprint { get; private set; }
        internal ThirdPartyExportBuilder(TypeBlueprint blueprint)
        {
            Blueprint = blueprint;
            if (blueprint.ThirdParty == null)
            {
                blueprint.ThirdParty = new TsThirdPartyAttribute(Type.FullName);
            }
        }

        /// <summary>
        /// Gets type that is being configured for export
        /// </summary>
        public Type Type
        {
            get { return Blueprint.Type; }
        }

        internal TsThirdPartyAttribute Attr
        {
            get { return Blueprint.ThirdParty; }
        }
    }

    /// <summary>
    /// Set of extensions for type export configuration
    /// </summary>
    public static partial class TypeConfigurationBuilderExtensions
    {
       
        /// <summary>
        ///     Makes RT to treat specified type as type from third-party library
        /// </summary>
        /// <typeparam name="T">Type to include</typeparam>
        /// <param name="builder">Configuration builder</param>
        /// <returns>Fluent</returns>
        public static ThirdPartyExportBuilder<T> ExportAsThirdParty<T>(this ConfigurationBuilder builder)
        {
            var bp = builder.GetCheckedThirdPartyBlueprint(typeof(T));

            if (builder.TypeExportBuilders.ContainsKey(typeof(T)))
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(typeof(T), "third party");
            }

            var conf =
                builder.ThirdPartyBuilders.GetOrCreate(typeof(T), () => new ThirdPartyExportBuilder<T>(bp))
                    as ThirdPartyExportBuilder<T>;
            if (conf == null)
            {
                ErrorMessages.RTE0017_FluentContradict.Throw(typeof(T), "third party");
            }


            return conf;

        }

        /// <summary>
        ///     Makes RT to treat specified types as types from third-party library
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="types">Types to include</param>
        /// <param name="configuration">Configuration to be applied to each type</param>
        /// <returns>Fluent</returns>
        public static void ExportAsThirdParty(this ConfigurationBuilder builder, IEnumerable<Type> types,
            Action<ThirdPartyExportBuilder> configuration = null)
        {
            foreach (var type in types)
            {
                if (builder.TypeExportBuilders.ContainsKey(type))
                {
                    ErrorMessages.RTE0017_FluentContradict.Throw(type, "third party");
                }
                var conf = builder.ThirdPartyBuilders.GetOrCreate(type, () =>
                {
                    var bp = builder.GetCheckedThirdPartyBlueprint(type);
                    if (!type._IsGenericTypeDefinition())
                    {
                        var t = typeof(ThirdPartyExportBuilder<>).MakeGenericType(type);
                        return (ThirdPartyExportBuilder)Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { bp }, null);
                    }

                    return new ThirdPartyExportBuilder(bp);
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