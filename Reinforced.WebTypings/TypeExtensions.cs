using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reinforced.WebTypings
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static Type GetArg(this Type t)
        {
            return t.GetGenericArguments()[0];
        }
        public static bool IsDictionary(this Type t)
        {
            if (t.IsGenericType)
            {
                var tg = t.GetGenericTypeDefinition();
                
                if (typeof (IDictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof (IReadOnlyDictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof (Dictionary<,>).IsAssignableFrom(tg)) return true;
                if (typeof(IDictionary).IsAssignableFrom(t)) return true;
            }
            else
            {
                if (typeof(IDictionary).IsAssignableFrom(t)) return true;
            }
            return false;
        }
        public static bool IsEnumerable(this Type t)
        {
            if (t.IsArray) return true;
            if (t.IsGenericType)
            {
                var tg = t.GetGenericTypeDefinition();
                if (typeof (IEnumerable<>).IsAssignableFrom(tg)) return true;
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

        public static bool IsIgnored(this MemberInfo t)
        {
            return t.GetCustomAttribute<TsIgnoreAttribute>() != null;
        }

        public static bool IsIgnored(this ParameterInfo t)
        {
            return t.GetCustomAttribute<TsIgnoreAttribute>() != null;
        }

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