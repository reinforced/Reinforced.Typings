using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Exceptions;
// ReSharper disable CheckNamespace
// ReSharper disable PossibleNullReferenceException

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Set of .With-extensions
    /// </summary>
    public static partial class WithExtensions
    {
        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="field">Field to include</param>
        /// <returns>Fluent</returns>
        public static FieldExportBuilder WithField<T, TData>(this ITypedExportBuilder<T> tc,
            Expression<Func<T, TData>> field)
        {
            var prop = LambdaHelpers.ParseFieldLambda(field);
            ClassOrInterfaceExportBuilder tcb = tc as ClassOrInterfaceExportBuilder;
            return new FieldExportBuilder(tcb.Blueprint, prop);
        }

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="fieldName">Name of field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public static T WithField<T>(this T tc, string fieldName, Action<FieldExportBuilder> configuration)
            where T : ClassOrInterfaceExportBuilder
        {
            var field = tc.Blueprint.Type._GetField(fieldName);
            if (field == null)
            {
                ErrorMessages.RTE0013_InvalidField.Throw(fieldName, tc.Blueprint.Type.FullName);
            }
            tc.WithFields(new[] { field }, configuration);
            return tc;
        }

        

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, Func<FieldInfo, bool> predicate,
            Action<FieldExportBuilder> configuration = null) where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers((t, b) => t._GetFields(b))
                .Where(predicate);
            tc.WithFields(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include all fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithAllFields<T>(this T tc, Action<FieldExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers((t, b) => t._GetFields(b));
            tc.WithFields(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include all public fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithPublicFields<T>(this T tc, Action<FieldExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop =
                tc.Blueprint
                    .GetExportingMembers((t, b) => t._GetFields(b), true);
            tc.WithFields(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include specified fields to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing fields to include</param>
        /// <param name="configuration">Configuration to be applied to each field</param>
        /// <returns>Fluent</returns>
        public static T WithFields<T>(this T tc, BindingFlags bindingFlags,
            Action<FieldExportBuilder> configuration = null) where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.Type._GetFields(bindingFlags);
            tc.WithFields(prop, configuration);
            return tc;
        }
    }
}
