using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Fluent.Interfaces;

namespace Reinforced.Typings.Fluent
{
    public static partial class ConfigurationBuildersExtensions
    {
        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodConfigurationBuilder WithMethod<T, TData>(this TypeConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = new MethodConfigurationBuilder(prop, tc._blueprint);
            ExtractParameters(tcb, method);
            return methodConf;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithMethod<T, TData>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithMethod<T, TData>(this ClassConfigurationBuilder<T> tc,
            Expression<Func<T, TData>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodConfigurationBuilder WithMethod<T>(this TypeConfigurationBuilder<T> tc,
            Expression<Action<T>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ITypeConfigurationBuilder tcb = tc;
            var methodConf = new MethodConfigurationBuilder(prop, tc._blueprint);
            ExtractParameters(tcb, method);
            return methodConf;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static InterfaceConfigurationBuilder<T> WithMethod<T>(this InterfaceConfigurationBuilder<T> tc,
            Expression<Action<T>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">Configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public static ClassConfigurationBuilder<T> WithMethod<T>(this ClassConfigurationBuilder<T> tc,
            Expression<Action<T>> method, Action<MethodConfigurationBuilder> configuration)
        {
            tc.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ITypeConfigurationBuilder tcb = tc;
            ExtractParameters(tcb, method);
            return tc;
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="methods">Methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, IEnumerable<MethodInfo> methods,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            return ApplyMethodsConfiguration(tc, methods, configuration);
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Context.Project.Blueprint(tc.Type).GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetMethods(b), tc.FlattenLimiter).Where(predicate);
            return tc.WithMethods(prop, configuration);
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags,
            Action<MethodConfigurationBuilder> configuration = null) where T : ITypeConfigurationBuilder
        {
            var prop = tc.Context.Project.Blueprint(tc.Type).GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetMethods(bindingFlags), tc.FlattenLimiter);
            return tc.WithMethods(prop, configuration);
        }


        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithAllMethods<T>(this T tc, Action<MethodConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop = tc.Context.Project.Blueprint(tc.Type).GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetMethods(b), tc.FlattenLimiter);
            return tc.WithMethods(prop, configuration);
        }

        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithPublicMethods<T>(this T tc, Action<MethodConfigurationBuilder> configuration = null)
            where T : ITypeConfigurationBuilder
        {
            var prop =
                tc.Context.Project.Blueprint(tc.Type).GetExportingMembers(tc.IsHierarchyFlatten, (t, b) => t._GetMethods(b), tc.FlattenLimiter, true);
            return tc.WithMethods(prop, configuration);
        }
    }
}
