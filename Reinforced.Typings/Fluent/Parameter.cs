using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    /// Shortcut for method parameters mocking
    /// </summary>
    public class Ts
    {
        /// <summary>
        /// Parameter mock for specified type
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <returns>Mock</returns>
        public static T Parameter<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Parameter mock with parameter configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static T Parameter<T>(Action<ParameterConfigurationBuilder> configuration)
        {
            return default(T);
        }

        internal static MethodInfo ParametrizedParameterMethod;

        static Ts()
        {
            Expression<Func<object>> lambda = () => Parameter<object>(a => a.Ignore());
            ParametrizedParameterMethod = LambdaHelpers.ParseMethodLambda(lambda).GetGenericMethodDefinition();
        }
    }
}
