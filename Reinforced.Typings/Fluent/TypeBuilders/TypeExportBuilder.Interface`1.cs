using System;
using System.Linq.Expressions;
// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Fluent export configuration builder for interface (generic)
    /// </summary>
    public class InterfaceExportBuilder<T> : InterfaceExportBuilder, ITypedExportBuilder<T>
    {
        internal InterfaceExportBuilder(TypeBlueprint blueprint) : base(blueprint)
        {
        }
        
        /// <summary>
        ///     Include specified field to resulting typing
        /// </summary>
        /// <param name="field">Field to include</param>
        /// <param name="configuration">Configuration to be applied to selected field</param>
        /// <returns>Fluent</returns>
        public InterfaceExportBuilder<T> WithField<TData>(Expression<Func<T, TData>> field, Action<PropertyExportBuilder> configuration)
        {
            ApplyMembersConfiguration(new[] { LambdaHelpers.ParseFieldLambda(field) }, configuration);
            return this;
        }

        /// <summary>
        ///     Include specified method to resulting typing.
        ///     User <see cref="Ts.Parameter{T}()" /> to mock up method parameters or specify configuration for perticular method
        ///     parameter
        /// </summary>
        /// <param name="method">Method to include</param>
        /// <param name="configuration">configuration to be applied to method</param>
        /// <returns>Fluent</returns>
        public InterfaceExportBuilder<T> WithMethod<TData>(Expression<Func<T, TData>> method, Action<MethodExportBuilder> configuration)
        {
            this.WithMethods(new[] { LambdaHelpers.ParseMethodLambda(method) }, configuration);
            ExtractParameters(method);
            return this;
        }

        /// <summary>
        ///     Include specified property to resulting typing
        /// </summary>
        /// <param name="property">Property to include</param>
        /// <param name="configuration">Configuration to be applied to selected property</param>
        /// <returns>Fluent</returns>
        public InterfaceExportBuilder<T> WithProperty<TData>(Expression<Func<T, TData>> property, Action<PropertyExportBuilder> configuration)
        {
            WithProperties(new[] { LambdaHelpers.ParsePropertyLambda(property) }, configuration);
            return this;
        }
    }
}