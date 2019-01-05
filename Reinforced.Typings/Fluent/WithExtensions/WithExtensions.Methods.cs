using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
// ReSharper disable CheckNamespace
// ReSharper disable PossibleNullReferenceException

namespace Reinforced.Typings.Fluent
{
    public static partial class WithExtensions
    {
        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodExportBuilder WithMethod<T, TData>(this ITypedExportBuilder<T> tc,
            Expression<Func<T, TData>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ClassOrInterfaceExportBuilder tcb = tc as ClassOrInterfaceExportBuilder;
            var methodConf = new MethodExportBuilder(tcb.Blueprint, prop);
            tcb.ExtractParameters(method);
            return methodConf;
        }
        
        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="method">Method to include</param>
        /// <returns>Fluent</returns>
        public static MethodExportBuilder WithMethod<T>(this ITypedExportBuilder<T> tc,
            Expression<Action<T>> method)
        {
            var prop = LambdaHelpers.ParseMethodLambda(method);
            ClassOrInterfaceExportBuilder tcb = tc as ClassOrInterfaceExportBuilder;
            var methodConf = new MethodExportBuilder(tcb.Blueprint, prop);
            tcb.ExtractParameters(method);
            return methodConf;
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="predicate">Predicate function that should mathc for methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, Func<MethodInfo, bool> predicate,
            Action<MethodExportBuilder> configuration = null) where T : ClassOrInterfaceExportBuilder
        {
            var prop = tc.Blueprint.GetExportingMembers((t, b) => t._GetMethods(b))
                .Where(predicate);

            tc.WithMethods(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include specified methods to resulting typing.
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="bindingFlags">BindingFlags describing methods to include</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithMethods<T>(this T tc, BindingFlags bindingFlags,
            Action<MethodExportBuilder> configuration = null) where T : ClassOrInterfaceExportBuilder
        {
            var prop = 
                tc.Blueprint.GetExportingMembers((t, b) => t._GetMethods(bindingFlags));
             tc.WithMethods(prop, configuration);
            return tc;
        }


        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithAllMethods<T>(this T tc, Action<MethodExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop = 
                tc.Blueprint.GetExportingMembers((t, b) => t._GetMethods(b));
            tc.WithMethods(prop, configuration);
            return tc;
        }

        /// <summary>
        ///     Include all methods to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="configuration">Configuration to be applied to each method</param>
        /// <returns>Fluent</returns>
        public static T WithPublicMethods<T>(this T tc, Action<MethodExportBuilder> configuration = null)
            where T : ClassOrInterfaceExportBuilder
        {
            var prop =
                tc.Blueprint.GetExportingMembers((t, b) => t._GetMethods(b), true);
            tc.WithMethods(prop, configuration);
            return tc;
        }
    }
}
