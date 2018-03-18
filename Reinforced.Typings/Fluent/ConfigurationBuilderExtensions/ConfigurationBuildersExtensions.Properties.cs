using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Exceptions;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    public static partial class ConfigurationBuildersExtensions
    {

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <returns>Fluent</returns>
        public static PropertyExportConfigurationBuilder WithProperty<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property)
        {
            var prop = LambdaHelpers.ParsePropertyLambda(property);
            ITypeConfigurationBuilder tcb = tc;
            return
                (PropertyExportConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfigurationBuilder(prop));
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithProperty<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithProperty<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> property, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="propertyName">Name of property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static ITypeConfigurationBuilder WithProperty(this ITypeConfigurationBuilder tc, string propertyName,
            Action<PropertyExportConfigurationBuilder> configuration)
        {
            var prop = tc.Type._GetProperty(propertyName);
            if (prop == null)
            {
                ErrorMessages.RTE0013_InvalidProperty.Throw(propertyName, tc.Type.FullName);
            }
            return tc.WithProperties(new[] { prop }, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="properties">Properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, IEnumerable<PropertyInfo> properties,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMembersConfiguration(tc, properties, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function for properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetProperties(b), tc.FlattenLimiter).Where(predicate);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetProperties(bindingFlags), tc.FlattenLimiter);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include all properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithAllProperties<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetProperties(b), tc.FlattenLimiter);
            return tc.WithProperties(prop, configuration);
        }

        /// <summary>
        ///     Include all public properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithPublicProperties<T>(this T tc,
            Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type
                .GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetProperties(b), tc.FlattenLimiter, true);
            return tc.WithProperties(prop, configuration);
        }
    }
}
