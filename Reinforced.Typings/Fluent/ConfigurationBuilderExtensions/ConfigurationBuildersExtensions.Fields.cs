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
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <returns>Fluent</returns>
        public static PropertyExportConfigurationBuilder WithField<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> field)
        {
            var prop = LambdaHelpers.ParseFieldLambda(field);
            ITypeConfigurationBuilder tcb = tc;
            return
                (PropertyExportConfigurationBuilder)
                tcb.MembersConfiguration.GetOrCreate(prop, () => new PropertyExportConfigurationBuilder(prop));
        }

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithField<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> field, Action<PropertyExportConfigurationBuilder> configuration)
        {
            ApplyMembersConfiguration(tc, new[] { LambdaHelpers.ParseFieldLambda(field) }, configuration);
            return tc;
        }

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithField<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> field, Action<PropertyExportConfigurationBuilder> configuration)
        {
            return tc.WithFields(new[] { LambdaHelpers.ParseFieldLambda(field) }, configuration);
        }

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="fieldName">Name of field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public static ITypeConfigurationBuilder WithField(this ITypeConfigurationBuilder tc, string fieldName,
            Action<PropertyExportConfigurationBuilder> configuration)
        {
            var field = tc.Type._GetField(fieldName);
            if (field == null)
            {
                ErrorMessages.RTE0012_InvalidField.Throw(fieldName, tc.Type.FullName);
            }
            return tc.WithFields(new[] { field }, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="fields">Fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, IEnumerable<FieldInfo> fields,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMembersConfiguration(tc, fields, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetFields(b), tc.FlattenLimiter).Where(predicate);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include all fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithAllFields<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetFields(b), tc.FlattenLimiter);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include all public fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithPublicFields<T>(this T tc, Action<PropertyExportConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop =
                tc.Type.GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetFields(b), tc.FlattenLimiter, true);
            return tc.WithFields(prop, configuration);
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, BindingFlags bindingFlags,
            Action<PropertyExportConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Type._GetFields(bindingFlags);
            return tc.WithFields(prop, configuration);
        }
    }
}
