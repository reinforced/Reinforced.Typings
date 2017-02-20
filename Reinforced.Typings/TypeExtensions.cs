using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reinforced.Typings.Ast;
using Reinforced.Typings.Attributes;

namespace Reinforced.Typings
{
    /// <summary>
    ///     Useful extensions for reflection
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Binding flags for searching all members
        /// </summary>
        public const BindingFlags MembersFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        /// <summary>
        ///     Search predicate to exclude ignored and compiler-generated items
        /// </summary>
        public static readonly Func<MemberInfo, bool> TypeScriptMemberSearchPredicate =
            c =>
                (!ConfigurationRepository.Instance.IsIgnored(c)) &&
                (c.GetCustomAttribute<CompilerGeneratedAttribute>() == null);

        /// <summary>
        ///     Returns all properties that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Properties array</returns>
        public static PropertyInfo[] GetExportedProperties(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedProperties(t);
        }

        /// <summary>
        ///     Returns all fields that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Fields array</returns>
        public static FieldInfo[] GetExportedFields(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedFields(t);
        }

        /// <summary>
        ///     Returns all methods that should be exported to TypeScript for specified type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Methods array</returns>
        public static MethodInfo[] GetExportedMethods(this Type t)
        {
            return ConfigurationRepository.Instance.GetExportedMethods(t);
        }

        /// <summary>
        ///     Determines is type derived from Nullable or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is nullable value type. False otherwise</returns>
        public static bool IsNullable(this Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        ///     Determines is type is static
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is static. False otherwise</returns>
        public static bool IsStatic(this Type t)
        {
            return (t.IsAbstract && t.IsSealed);
        }

        /// <summary>
        ///     Determines is member static or not
        /// </summary>
        /// <param name="member">Type member</param>
        /// <returns>True if member is static. False otherwise</returns>
        public static bool IsStatic(this MemberInfo member)
        {
            if (member is PropertyInfo) return IsStatic((PropertyInfo)member);
            if (member is MethodInfo) return ((MethodInfo)member).IsStatic;
            if (member is FieldInfo) return ((FieldInfo)member).IsStatic;
            return false;
        }

        /// <summary>
        ///     Determines is property static or not
        /// </summary>
        /// <param name="propertyInfo">Property</param>
        /// <returns>True if member is static. False otherwise</returns>
        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetMethod != null)
            {
                return propertyInfo.GetMethod.IsStatic;
            }

            if (propertyInfo.SetMethod != null)
            {
                return propertyInfo.SetMethod.IsStatic;
            }
            return false;
        }

        /// <summary>
        ///     Retrieves first type argument of type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>First type argument</returns>
        public static Type GetArg(this Type t)
        {
            return t.GetGenericArguments()[0];
        }

        /// <summary>
        ///     Determines if type is Dictionary-like
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
        ///     Determines if type is enumerable regardless of generic spec
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is enumerable (incl. array type). False otherwise.</returns>
        public static bool IsEnumerable(this Type t)
        {
            if (t.IsArray) return true;
            if (typeof(IEnumerable).IsAssignableFrom(t)) return true;
            if (t.IsGenericType)
            {
                var tg = t.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>).IsAssignableFrom(tg)) return true;
            }
            return false;
        }

        /// <summary>
        ///     Determines if supplied type is non-generic enumerable
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if supplied type is nongeneric enumerable. False otherwise</returns>
        public static bool IsNongenericEnumerable(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return false;
            var interfaces = t.GetInterfaces();
            var containsEnumerable = interfaces.Contains(typeof(IEnumerable));
            var containsGenericEnumerable =
                interfaces.Any(c => c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return containsEnumerable && !containsGenericEnumerable;
        }

        /// <summary>
        ///     Determines should type be exported as interface or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public static bool IsExportingAsInterface(this Type t)
        {
            return ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t) != null;
        }

        /// <summary>
        ///     Determines should type be exported as class or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type should be exported as interface. False otherwise</returns>
        public static bool IsExportingAsClass(this Type t)
        {
            return ConfigurationRepository.Instance.ForType<TsClassAttribute>(t) != null;
        }

        /// <summary>
        ///     Determines if type member should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type member info</param>
        /// <returns>True if type member should be ignored, false otherwise</returns>
        public static bool IsIgnored(this MemberInfo t)
        {
            return ConfigurationRepository.Instance.IsIgnored(t);
        }

        /// <summary>
        ///     Determines if parameter should be ignored for translation using corresponding Typings attribute
        /// </summary>
        /// <param name="t">Parameter info</param>
        /// <returns>True if parameter should be ignored, false otherwise</returns>
        public static bool IsIgnored(this ParameterInfo t)
        {
            return ConfigurationRepository.Instance.IsIgnored(t);
        }

        /// <summary>
        ///     Removes generics postfix (all text after '`') from typename
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Clean, genericless name</returns>
        private static string CleanGenericName(this Type t)
        {
            if (t.IsGenericType)
            {
                var name = t.Name;
                var qidx = name.IndexOf('`');
                return name.Substring(0, qidx);
            }
            return t.Name;
        }

        /// <summary>
        ///     Retrieves parameter name from corresponding attribute. If attribute not present then takes parameter name via
        ///     reflection
        /// </summary>
        /// <param name="element">Parameter info</param>
        /// <returns>Parameter name</returns>
        public static string GetName(this ParameterInfo element)
        {
            var fa = ConfigurationRepository.Instance.ForMember(element);
            if (fa != null && !string.IsNullOrEmpty(fa.Name)) return fa.Name;
            return element.Name;
        }

        /// <summary>
        ///     Determines if supplied type is delegate type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type is delegate, false otherwise</returns>
        public static bool IsDelegate(this Type t)
        {
            if (t.BaseType == null) return false;
            return typeof(MulticastDelegate).IsAssignableFrom(t.BaseType);
        }

        private static RtTypeName[] SerializeGenericArguments(this Type t)
        {
            if (!t.IsGenericTypeDefinition) return new RtTypeName[0];
            if (t.IsGenericTypeDefinition)
            {
                // arranged generic attribute means that generic type is replaced with real one
                var args = t.GetGenericArguments().Where(g => g.GetCustomAttribute<TsGenericAttribute>() == null);
                var argsStr = args.Select(c => new RtSimpleTypeName(c.Name)).Cast<RtTypeName>().ToArray();
                return argsStr;
            }
            return new RtTypeName[0];
        }

        /// <summary>
        ///     Retrieves type name from type itself or from corresponding Reinforced.Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="genericArguments">Generic arguments to be substituted to type</param>
        /// <returns>Type name</returns>
        public static RtSimpleTypeName GetName(this Type t, RtTypeName[] genericArguments = null)
        {
            if (t.IsEnum)
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                var ns = t.Name;
                if (te != null && !string.IsNullOrEmpty(te.Name))
                {
                    ns = te.Name;
                }
                return new RtSimpleTypeName(ns);
            }

            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            var nameFromAttr = tc != null ? tc.Name : ti.Name;
            var name = (!string.IsNullOrEmpty(nameFromAttr) ? nameFromAttr : t.CleanGenericName());
            if (genericArguments == null) genericArguments = t.SerializeGenericArguments();

            if (ti != null)
            {
                if (ti.AutoI && !name.StartsWith("I")) name = "I" + name;
            }
            return new RtSimpleTypeName(name, genericArguments);
        }

        /// <summary>
        ///     Retrieves type namespace from type itself or from corresponding Typings attribute
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="distinguishAutoTypes">
        ///     Forces GetNamespace to return "-" for interfaces with IncludeInterface = false and
        ///     null for anonymous types
        /// </param>
        /// <returns>Full-qualified namespace name</returns>
        public static string GetNamespace(this Type t, bool distinguishAutoTypes = false)
        {
            if (t.IsEnum)
            {
                var te = ConfigurationRepository.Instance.ForType<TsEnumAttribute>(t);
                var ns = t.Namespace;
                if (te != null)
                {
                    if (!te.IncludeNamespace)
                    {
                        ns = distinguishAutoTypes ? "-" : string.Empty;
                    }
                    else if (!string.IsNullOrEmpty(te.Namespace))
                    {
                        ns = te.Namespace;
                    }
                }
                return ns;
            }
            var tc = ConfigurationRepository.Instance.ForType<TsClassAttribute>(t);
            var ti = ConfigurationRepository.Instance.ForType<TsInterfaceAttribute>(t);
            if (tc == null && ti == null) return t.Namespace;
            var nameFromAttr = tc != null ? tc.Namespace : ti.Namespace;
            var includeNamespace = tc != null ? tc.IncludeNamespace : ti.IncludeNamespace;
            if (!includeNamespace) return distinguishAutoTypes ? "-" : string.Empty;
            if (!string.IsNullOrEmpty(nameFromAttr)) return nameFromAttr;
            return t.Namespace;
        }

        /// <summary>
        ///     Returns access modifier for specified field
        /// </summary>
        /// <param name="fieldInfo">Field</param>
        /// <returns>Access modifier string</returns>
        public static AccessModifier GetModifier(this FieldInfo fieldInfo)
        {
            if (fieldInfo.IsPrivate) return AccessModifier.Private;
            if (fieldInfo.IsFamily) return AccessModifier.Protected;
            return AccessModifier.Public;
        }

        /// <summary>
        ///     Returns access modifier for specified method
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Access modifier string</returns>
        public static AccessModifier GetModifier(this MethodInfo methodInfo)
        {
            if (methodInfo.IsPrivate) return AccessModifier.Private;
            if (methodInfo.IsFamily) return AccessModifier.Protected;
            return AccessModifier.Public;
        }

        /// <summary>
        ///     Returns access modifier for specified constructor
        /// </summary>
        /// <param name="constructorInfo">Constructor</param>
        /// <returns>Access modifier string</returns>
        public static AccessModifier GetModifier(this ConstructorInfo constructorInfo)
        {
            if (constructorInfo.IsPrivate) return AccessModifier.Private;
            if (constructorInfo.IsFamily) return AccessModifier.Protected;
            return AccessModifier.Public;
        }

        /// <summary>
        ///     Returns access modifier for specified constructor
        /// </summary>
        /// <param name="propertyInfo">Property</param>
        /// <returns>Access modifier string</returns>
        public static AccessModifier GetModifier(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetMethod == null)
            {
                return GetModifier(propertyInfo.SetMethod);
            }

            if (propertyInfo.SetMethod == null)
            {
                return GetModifier(propertyInfo.GetMethod);
            }
            var getAccessor = GetModifier(propertyInfo.GetMethod);
            var setAccessor = GetModifier(propertyInfo.SetMethod);

            return getAccessor > setAccessor ? getAccessor : setAccessor;
        }

        /// <summary>
        ///     Returns access modifier for specified type member
        /// </summary>
        /// <param name="member">Type member</param>
        /// <returns>Access modifier string</returns>
        public static AccessModifier GetModifier(this MemberInfo member)
        {
            if (member is PropertyInfo) return GetModifier((PropertyInfo)member);
            if (member is MethodInfo) return GetModifier((MethodInfo)member);
            if (member is FieldInfo) return GetModifier((FieldInfo)member);
            return AccessModifier.Public;
        }

        /// <summary>
        ///     Determines if propercy is "bounced".
        ///     It means property with different accesor's access level
        /// </summary>
        /// <param name="propertyInfo">Property</param>
        /// <returns>True if property has different access levels for accessor</returns>
        public static bool IsBounceProperty(this PropertyInfo propertyInfo)
        {
            var g = propertyInfo.GetMethod;
            var s = propertyInfo.SetMethod;
            if ((g == null) || (s == null)) return true;
            return
                g.IsPublic != s.IsPublic
                || g.IsFamily != s.IsFamily
                || g.IsPrivate != s.IsPrivate;
        }

        internal static MethodBase GetMethodWithSameParameters(MethodBase[] methodsSet, Type[] parameters)
        {
            MethodBase result = null;
            foreach (var method in methodsSet)
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length != parameters.Length) continue;

                var parametersMatch = true;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] != methodParams[i].ParameterType)
                    {
                        parametersMatch = false;
                        break;
                    }
                }
                if (parametersMatch)
                {
                    result = method;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        ///     Converts AccessModifier to corresponding TypeScript source text
        /// </summary>
        /// <param name="modifier">Access modifier</param>
        /// <returns>Access modifier text</returns>
        public static string ToModifierText(this AccessModifier modifier)
        {
            switch (modifier)
            {
                case AccessModifier.Private:
                    return "private";
                case AccessModifier.Protected:
                    return "protected";
            }
            return "public";
        }

        /// <summary>
        ///     Converts AccessModifier to corresponding TypeScript source text
        /// </summary>
        /// <param name="modifier">Access modifier</param>
        /// <returns>Access modifier text</returns>
        public static string ToModifierOmitPublic(this AccessModifier modifier)
        {
            switch (modifier)
            {
                case AccessModifier.Private:
                    return "private";
                case AccessModifier.Protected:
                    return "protected";
            }
            return string.Empty;
        }
    }
}