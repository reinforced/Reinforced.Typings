using System;
using System.Linq.Expressions;
using System.Reflection;
using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings.Fluent
{
    /// <summary>
    ///     Set of helper reflection methods
    /// </summary>
    internal static class LambdaHelpers
    {
        /// <summary>
        ///     Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static PropertyInfo ParsePropertyLambda<T1, T2>(Expression<Func<T1, T2>> lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) ErrorMessages.RTE0010_PropertyLambdaExpected.Throw(lambda.ToString());
            var pi = mex.Member as PropertyInfo;
            if (pi == null) ErrorMessages.RTE0010_PropertyLambdaExpected.Throw(lambda.ToString());
            return pi;
        }

        /// <summary>
        ///     Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static PropertyInfo ParsePropertyLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) ErrorMessages.RTE0010_PropertyLambdaExpected.Throw(lambda.ToString());
            var pi = mex.Member as PropertyInfo;
            if (pi == null) ErrorMessages.RTE0010_PropertyLambdaExpected.Throw(lambda.ToString());
            return pi;
        }

        /// <summary>
        ///     Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <typeparam name="T1">T1</typeparam>
        /// <typeparam name="T2">T2</typeparam>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static FieldInfo ParseFieldLambda<T1, T2>(Expression<Func<T1, T2>> lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) ErrorMessages.RTE0011_FieldLambdaExpected.Throw(lambda.ToString());
            var pi = mex.Member as FieldInfo;
            if (pi == null) ErrorMessages.RTE0011_FieldLambdaExpected.Throw(lambda.ToString());
            return pi;
        }

        /// <summary>
        ///     Parses supplied lambda expression and retrieves PropertyInfo from it
        /// </summary>
        /// <param name="lambda">Property Lambda expression</param>
        /// <returns>PropertyInfo referenced by this expression</returns>
        public static FieldInfo ParseFieldLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MemberExpression;
            if (mex == null) ErrorMessages.RTE0011_FieldLambdaExpected.Throw(lambda.ToString());
            var pi = mex.Member as FieldInfo;
            if (pi == null) ErrorMessages.RTE0011_FieldLambdaExpected.Throw(lambda.ToString());
            return pi;
        }

        /// <summary>
        ///     Parses supplied lambda expression and retrieves MethodInfo from it
        /// </summary>
        /// <param name="lambda">Method lambda expression</param>
        /// <returns>MethodInfo referenced by this expression</returns>
        public static MethodInfo ParseMethodLambda(LambdaExpression lambda)
        {
            var mex = lambda.Body as MethodCallExpression;
            if (mex == null) ErrorMessages.RTE0008_FluentWithMethodError.Throw();
            return mex.Method;
        }

        /// <summary>
        ///     Parses supplied lambda expression and retrieves ConstructorInfo from it
        /// </summary>
        /// <param name="lambda">Constructor lambda expression ( => new Obejct(Ts.Parameter...))</param>
        /// <returns>Constructor referenced by this expression</returns>
        public static ConstructorInfo ParseConstructorLambda(LambdaExpression lambda)
        {
            var nex = lambda.Body as NewExpression;
            if (nex == null) ErrorMessages.RTE0012_NewExpressionLambdaExpected.Throw();
            return nex.Constructor;
        }
    }
}