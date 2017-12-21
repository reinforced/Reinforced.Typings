using System;
using Reinforced.Typings.Ast.TypeNames;

namespace Reinforced.Typings
{
    /// <summary>
    /// Class holding inline type inferers for specified node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InlineTypeInferers<T>
    {
        internal Type StrongType { get; set; }
        internal string Type { get; set; }

        private Func<T, TypeResolver, string> _stringResolver;
        private Func<T, TypeResolver, RtTypeName> _typenameResolver;
        private Func<T, string> _stringSimpleResolver;
        private Func<T, RtTypeName> _typenameSimpleResolver;

        internal Func<T, TypeResolver, string> StringResolver
        {
            get { return _stringResolver; }
            set
            {
                _stringResolver = value;
                _typenameResolver = null;
                _stringSimpleResolver = null;
                _typenameSimpleResolver = null;
            }
        }

        internal Func<T, TypeResolver, RtTypeName> TypenameResolver
        {
            get { return _typenameResolver; }
            set
            {
                _typenameResolver = value;
                _stringResolver = null;
                _stringSimpleResolver = null;
                _typenameSimpleResolver = null;
            }
        }

        internal Func<T, string> StringSimpleResolver
        {
            get { return _stringSimpleResolver; }
            set
            {
                _stringSimpleResolver = value;
                _typenameResolver = null;
                _stringResolver = null;
                _typenameSimpleResolver = null;
            }
        }

        internal Func<T, RtTypeName> TypenameSimpleResolver
        {
            get { return _typenameSimpleResolver; }
            set
            {
                _typenameSimpleResolver = value;
                _stringSimpleResolver = null;
                _stringResolver = null;
                _typenameResolver = null;
            }
        }

        internal RtTypeName Infer(T src, TypeResolver resolver)
        {
            if (StringResolver != null)
            {
                var r = StringResolver(src, resolver);
                return new RtSimpleTypeName(r);
            }

            if (TypenameSimpleResolver != null)
            {
                var r = TypenameSimpleResolver(src);
                return r;
            }

            if (StringSimpleResolver != null)
            {
                var r = StringSimpleResolver(src);
                return new RtSimpleTypeName(r);
            }

            if (TypenameResolver != null)
            {
                var r = TypenameResolver(src, resolver);
                return r;
            }
            return null;
        }
    }

    /// <summary>
    /// Decorates member that supports inline type inferring
    /// </summary>
    /// <typeparam name="T">Member Type</typeparam>
    public interface ISupportsInferring<T>
    {
        /// <summary>
        /// Type inferers set instance
        /// </summary>
        InlineTypeInferers<T> TypeInferers { get; }
    }
}