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
    /// Set of helper reflection methods
    /// </summary>
    internal static class LambdaHelpers
    {
        
        /// <summary>
        /// Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static PropertyInfo ParsePropertyLambda<T1, T2>(Expression<Func<T1, T2>> lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) throw new Exception("Here should be property");
            var pi = mex.Member as PropertyInfo;
            if (pi == null) throw new Exception("Here should be property");
            return pi;
        }

        /// <summary>
        /// Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static PropertyInfo ParsePropertyLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) throw new Exception("Here should be property");
            var pi = mex.Member as PropertyInfo;
            if (pi == null) throw new Exception("Here should be property");
            return pi;
        }

        /// <summary>
        /// Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static FieldInfo ParseFieldLambda<T1, T2>(Expression<Func<T1, T2>> lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) throw new Exception("Here should be field");
            var pi = mex.Member as FieldInfo;
            if (pi == null) throw new Exception("Here should be field");
            return pi;
        }

        /// <summary>
        /// Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static FieldInfo ParseFieldLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) throw new Exception("Here should be field");
            var pi = mex.Member as FieldInfo;
            if (pi == null) throw new Exception("Here should be field");
            return pi;
        }

        /// <summary>
        /// Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static MethodInfo ParseMethodLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MethodCallExpression;
            if (mex == null) throw new Exception("MethodCallExpression should be provided for .WithMethod call. Please use only lamba expressions in this place.");
            return mex.Method;
        }
    }
}
