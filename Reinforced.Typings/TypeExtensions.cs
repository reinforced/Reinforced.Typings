using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    /// <summary>
    /// Useful extensions for reflection
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines is type derived from Nullable or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is nullable value type. False otherwise</returns>
        public static bool IsNullable(this Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Retrieves first type argument of type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>First type argument</returns>
        public static Type GetArg(this Type t)
        {
            return t.GetGenericArguments()[0];
        }

        /// <summary>
        /// Determines if type is Dictionary-like
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is derived from dictionary type</returns>
        public static bool IsDictionary(this Type t)
        {
            if (t.IsGenericType)
            {
                var tg = t.GetGenericTypeDefinition();

                if (typeof(IDictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof(IReadOnlyDictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof(Dictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof(IDictionary).IsAssignableFrom(t)) return true;
            }
            else
            {
                if (typeof(IDictionary).IsAssignableFrom(t)) return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if type is enumerable regardless of generic spec
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is enumerable (incl. array type). False otherwise.</returns>
        public static bool IsEnumerable(this Type t)
        {
            if (t.IsArray) return true;
            if (t.IsGenericType)
            {
                var tg = t.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>).IsAssignableFrom(tg)) return true;
                if (typeof(IEnumerable).IsAssignableFrom(t)) return true;

            }
            else
            {
                if (typeof(IEnumerable).IsAssignableFrom(t)) return true;
            }
            return false;
        }

        public static bool IsNongenericEnumerable(this Type t)
        {
            return (typeof(IEnumerable).IsAssignableFrom(t));
        }

        public static bool IsExportingAsInterface(this Type t)
        {
            return t.GetCustomAttribute<TsInterfaceAttribute>() != null;
        }

        /// <summary>
        /// Determines if type member should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type member info</param>
        /// <returns>True if type member should be ignored, false otherwise</returns>
        public static bool IsIgnored(this MemberInfo t)
        {
            return t.GetCustomAttribute<TsIgnoreAttribute>() != null;
        }

        /// <summary>
        /// Determines if parameter should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Parameter info</param>
        /// <returns>True if parameter should be ignored, false otherwise</returns>
        public static bool IsIgnored(this ParameterInfo t)
        {
            return t.GetCustomAttribute<TsIgnoreAttribute>() != null;
        }

        /// <summary>
        /// Removes generics postfix (all text after '`') from typename
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Clean, genericless name</returns>
        private static string CleanGenericName(this Type t)
        {
            if (t.IsGenericType)
            {
                string name = t.Name;
                var qidx = name.IndexOf('`');
                return name.Substring(0, qidx);
            }
            return t.Name;
        }

        /// <summary>
        /// Retrieves parameter name from corresponding attribute. If attribute not present then takes parameter name via reflection
        /// </summary>
        /// <param name="element">Parameter info</param>
        /// <returns>Parameter name</returns>
        public static string GetName(this ParameterInfo element)
        {
            var fa = element.GetCustomAttribute<TsParameterAttribute>();
            if (fa != null && !string.IsNullOrEmpty(fa.Name)) return fa.Name;
            return element.Name;
        }

        public static bool IsDelegate(this Type t)
        {
            if (t.BaseType == null) return false;
            return typeof(MulticastDelegate).IsAssignableFrom(t.BaseType);
        }

        private static string SerializeGenericArguments(this Type t)
        {
            if (!t.IsGenericTypeDefinition) return String.Empty;
            if (t.IsGenericTypeDefinition)
            {
                var args = t.GetGenericArguments();
                string argsStr = String.Format("<{0}>", string.Join(", ", args.Select(c => c.Name)));
                return argsStr;
            }
            return String.Empty;
        }

        /// <summary>
        /// Retrieves type name from type itself or from corresponding Reinforced.Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Type name</returns>
        public static string GetName(this Type t)
        {
            if (t.IsEnum)
            {
                var te = t.GetCustomAttribute<TsEnumAttribute>();
                string ns = t.Name;
                if (te != null && !string.IsNullOrEmpty(te.Name))
                {
                    ns = te.Name;
                }
                return ns;
            }

            var tc = t.GetCustomAttribute<TsClassAttribute>();
            var ti = t.GetCustomAttribute<TsInterfaceAttribute>();
            string nameFromAttr = tc != null ? tc.Name : ti.Name;
            var name = (!string.IsNullOrEmpty(nameFromAttr) ? nameFromAttr : t.CleanGenericName()) + t.SerializeGenericArguments();
            if (ti != null)
            {
                if (ti.AutoI && !name.StartsWith("I")) name = "I" + name;
            }
            return name;
        }

        /// <summary>
        /// Retrieves type namespace from type itself or from corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Full-qualified namespace name</returns>
        public static string GetNamespace(this Type t)
        {
            if (t.IsEnum)
            {
                var te = t.GetCustomAttribute<TsEnumAttribute>();
                string ns = t.Namespace;
                if (te != null && te.IncludeNamespace && !string.IsNullOrEmpty(te.Namespace))
                {
                    ns = te.Namespace;
                }
                return ns;
            }
            var tc = t.GetCustomAttribute<TsClassAttribute>();
            var ti = t.GetCustomAttribute<TsInterfaceAttribute>();
            string nameFromAttr = tc != null ? tc.Namespace : ti.Namespace;
            if (!string.IsNullOrEmpty(nameFromAttr)) return nameFromAttr;
            return t.Namespace;
        }
    }
}