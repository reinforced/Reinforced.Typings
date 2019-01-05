using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace
// ReSharper disable PossibleNullReferenceException

namespace Reinforced.Typings.Fluent
{
    public static partial class WithExtensions
    {
        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <returns>Fluent</returns>
        public static PropertyExportBuilder WithProperty<T, TData>(this ITypedExportBuilder<T> tc,
            Expression<Func<T, TData>> property)
        {
            var prop = LambdaHelpers.ParsePropertyLambda(property);
            ClassOrInterfaceExportBuilder tcb = tc as ClassOrInterfaceExportBuilder;
            return new PropertyExportBuilder(tcb.Blueprint, prop);
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="propertyName">Name of property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public static T WithProperty<T>(this T tc, string propertyName,
            Action<PropertyExportBuilder> configuration) where T:ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.Type._GetProperty(propertyName);
            if (prop == null)
            {
                ErrorMessages.RTE0014_InvalidProperty.Throw(propertyName, tc.Blueprint.Type.FullName);
            }
            tc.WithProperties(new[] { prop }, configuration);
            return tc;
        }



        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function for properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, Func<PropertyInfo, bool> predicate,
            Action<PropertyExportBuilder> configuration = null) where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers( (t, b) => t._GetProperties(b)).Where(predicate);
            tc.WithProperties(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include specified properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing properties to include</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithProperties<T>(this T tc, BindingFlags bindingFlags,
            Action<PropertyExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers( (t, b) => t._GetProperties(bindingFlags));
            tc.WithProperties(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include all properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithAllProperties<T>(this T tc, Action<PropertyExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers( (t, b) => t._GetProperties(b));
            tc.WithProperties(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include all public properties to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each property</param>
        /// <returns>Fluent</returns>
        public static T WithPublicProperties<T>(this T tc,
            Action<PropertyExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint
                .GetExportingMembers( (t, b) => t._GetProperties(b), true);
            tc.WithProperties(prop, configuration);
            return tc;
        }
    }
}
