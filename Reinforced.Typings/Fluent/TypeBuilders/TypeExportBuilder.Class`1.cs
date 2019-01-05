using System;
using System.Linq.Expressions;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for class (generic)
    /// </summary>
    public class ClassExportBuilder<T> : ClassExportBuilder, ITypedExportBuilder<T>
    {
        internal ClassExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
        }

        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="field">Field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public ClassExportBuilder<T> WithField<TData>(Expression<Func<T, TData>> field, Action<PropertyExportBuilder> configuration)
        {
            ApplyMembersConfiguration(new[] { LambdaHelpers.ParseFieldLambda(field) }, configuration);
            return this;
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
        public ClassExportBuilder<T> WithMethod<TData>(Expression<Func<T, TData>> method, Action<MethodExportBuilder> configuration)
        {
            WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ExtractParameters(method);
            return this;
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
        public ClassExportBuilder<T> WithMethod(Expression<Action<T>> method, Action<MethodExportBuilder> configuration)
        {
            WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ExtractParameters(method);
            return this;
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="tc">Configuration builder</param>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public ClassExportBuilder<T> WithProperty<TData>(Expression<Func<T, TData>> property, Action<PropertyExportBuilder> configuration)
        {
            WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
            return this;
        }
    }
}