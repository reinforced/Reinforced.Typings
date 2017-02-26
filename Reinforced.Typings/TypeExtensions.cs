using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reinforced.Typings.Ast.TypeNames;
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
        ///     Determines is type derived from Nullable or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is nullable value type. False otherwise</returns>
        public static bool IsNullable(this Type t)
        {
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Determines if type is one of System.Tuple types set
        /// </summary>
        /// <param name="t">Type to check</param>
        /// <returns>True when type is tuple, false otherwise</returns>
        public static bool IsTuple(this Type t)
        {
            if (!t.IsGenericType) return false;
            var gen = t.GetGenericTypeDefinition();
            if (gen == typeof(System.Tuple<>)) return true;
            if (gen == typeof(System.Tuple<,>)) return true;
            if (gen == typeof(System.Tuple<,,>)) return true;
            if (gen == typeof(System.Tuple<,,,>)) return true;
            if (gen == typeof(System.Tuple<,,,,>)) return true;
            if (gen == typeof(System.Tuple<,,,,,>)) return true;
            if (gen == typeof(System.Tuple<,,,,,,>)) return true;
            if (gen == typeof(System.Tuple<,,,,,,,>)) return true;
            if (gen == typeof(System.Tuple<,,,,,,,>)) return true;
            return false;
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
        ///     Removes generics postfix (all text after '`') from typename
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Clean, genericless name</returns>
        internal static string CleanGenericName(this Type t)
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
        ///     Determines if supplied type is delegate type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type is delegate, false otherwise</returns>
        public static bool IsDelegate(this Type t)
        {
            if (t.BaseType == null) return false;
            return typeof(MulticastDelegate).IsAssignableFrom(t.BaseType);
        }

        internal static RtTypeName[] SerializeGenericArguments(this Type t)
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