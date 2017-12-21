using System;
using Reinforced.Typings.Ast.TypeNames;

// ReSharper disable CheckNamespace

namespace Reinforced.Typings.Fluent
{
    public static partial class TypingsConfigurationExtensions
    {
        /// <summary>
        /// Overrides type resolver for member type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Inferrable</param>
        /// <param name="inferrer">Type inferer</param>
        /// <returns>Fluent</returns>
        public static ISupportsInferring<T> InferType<T>(this ISupportsInferring<T> x, Func<T, TypeResolver, string> inferrer)
        {
            x.TypeInferers.StringResolver = inferrer;
            return x;
        }

        /// <summary>
        /// Overrides type resolver for member type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Inferrable</param>
        /// <param name="inferrer">Type inferer</param>
        /// <returns>Fluent</returns>
        public static ISupportsInferring<T> InferType<T>(this ISupportsInferring<T> x, Func<T, TypeResolver, RtTypeName> inferrer)
        {
            x.TypeInferers.TypenameResolver = inferrer;
            return x;
        }

        /// <summary>
        /// Overrides type resolver for member type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Inferrable</param>
        /// <param name="inferrer">Type inferer</param>
        /// <returns>Fluent</returns>
        public static ISupportsInferring<T> InferType<T>(this ISupportsInferring<T> x, Func<T, string> inferrer)
        {
            x.TypeInferers.StringSimpleResolver = inferrer;
            return x;
        }

        /// <summary>
        /// Overrides type resolver for member type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">Inferrable</param>
        /// <param name="inferrer">Type inferer</param>
        /// <returns>Fluent</returns>
        public static ISupportsInferring<T> InferType<T>(this ISupportsInferring<T> x, Func<T, RtTypeName> inferrer)
        {
            x.TypeInferers.TypenameSimpleResolver = inferrer;
            return x;
        }
    }
}
