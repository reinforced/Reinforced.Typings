using System;
using System.Collections.Generic;
using System.Reflection;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for class or interface
    /// </summary>
    public abstract class ClassOrInterfaceExportBuilder : TypeExportBuilder
    {
        internal ClassOrInterfaceExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="methods">Methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public void WithMethods(IEnumerable<MethodInfo> methods, Action<MethodExportBuilder> configuration = null)
        {
            ApplyMethodsConfiguration(methods, configuration);
        }


        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="properties">Properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public void WithProperties(IEnumerable<PropertyInfo> properties, Action<PropertyExportBuilder> configuration = null)
        {
            ApplyMembersConfiguration(properties, configuration);
        }


        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="fields">Fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public void WithFields(IEnumerable<FieldInfo> fields, Action<FieldExportBuilder> configuration = null)
        {
            ApplyMembersConfiguration(fields, configuration);
        }
    }

}