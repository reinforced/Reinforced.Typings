using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Shortcut for method parameters mocking
    /// </summary>
    public class Ts
    {

        internal static MethodInfo ParametrizedParameterMethod;

        static Ts()
        {
            Expression<Func<object>> lambda = () => Parameter<object>(a => a.Ignore());
            ParametrizedParameterMethod = LambdaHelpers.ParseMethodLambda(lambda).GetGenericMethodDefinition();
        }

        /// <summary>
        ///     Parameter mock for specified type
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <returns>Mock</returns>
        public static T Parameter<T>()
        {
            return default(T);
        }

        /// <summary>
        ///     Parameter mock with parameter configuration
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="configuration">Fluent parameter configuration</param>
        /// <returns>Mock</returns>
        public static T Parameter<T>(Action<ParameterConfigurationBuilder> configuration)
        {
            return default(T);
        }
    }
}