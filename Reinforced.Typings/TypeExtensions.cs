using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Reinforced.Typings.Ast.TypeNames;
using Reinforced.Typings.Attributes;
using Reinforced.Typings.Exceptions;

namespace Reinforced.Typings
{
    internal static class HashSetExtensions
    {
        internal static void AddIfNotExists<T>(this HashSet<T> hashSet, T val)
        {
            if (hashSet.Contains(val)) return;
            hashSet.Add(val);
        }


    }

    /// <summary>
    ///     Useful extensions for reflection
    /// </summary>
    public static class TypeExtensions
    {
        internal static object InstanceInternal(this Type t, params object[] parameters)
        {
#if NETCOREAPP1_0
            var dc = t.GetTypeInfo().DeclaredConstructors;
            var needed = dc.Where(d=>d.GetParameters().Length == parameters.Length).FirstOrDefault();
            return needed.Invoke(parameters);
#elif NETSTANDARD1_5
            var dc = t.GetTypeInfo().DeclaredConstructors;
            var needed = dc.Where(d=>d.GetParameters().Length == parameters.Length).FirstOrDefault();
            return needed.Invoke(parameters);
#elif NETSTANDARD1_6
            var dc = t.GetTypeInfo().DeclaredConstructors;
            var needed = dc.Where(d=>d.GetParameters().Length == parameters.Length).FirstOrDefault();
            return needed.Invoke(parameters);
#else
            return Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null);
#endif
        }

        internal static IEnumerable<Type> _GetTypes(this Assembly a, List<RtWarning> warnings)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var elo in e.LoaderExceptions)
                {
                    warnings.Add(ErrorMessages.RTW0008_TypeloadException.Warn(elo.Message));
                }
                return e.Types.Where(t => t != null);
            }

        }

        internal static T RetrieveOrCreateCustomAttribute<T>(this ICustomAttributeProvider member) where T : Attribute, new()
        {
            T proto = null;
            var attrs = member.GetCustomAttributes(true);
            foreach (var attribute in attrs)
            {
                if (typeof(T)._IsAssignableFrom(attribute.GetType()))
                {
                    proto = (T)attribute;
                }
            }
            if (proto == null) proto = new T();
            return proto;
        }

#if NETCORE
        internal static T GetCustomAttribute<T>(this Type t, bool inherit = true) where T : Attribute
        {
            return CustomAttributeExtensions.GetCustomAttribute<T>(t.GetTypeInfo(), inherit);
        }
        internal static IEnumerable<T> GetCustomAttributes<T>(this Type t, bool inherit = true) where T : Attribute
        {
            return CustomAttributeExtensions.GetCustomAttributes<T>(t.GetTypeInfo(), inherit);
        }
#endif
        
        public static bool IsReferenceForcedNullable(this MemberInfo member)
        {
            // non reference types will be converted to Nullable<> if they are nullable.
            // C# 8.0 nullable flag just changes the way reference types are handled. If enabled, then they are
            // not implicitly nullable anymore!
            Type memberType = member is PropertyInfo propertyInfo ? propertyInfo.PropertyType : (
                member is FieldInfo fieldInfo ? fieldInfo.FieldType : null
            );

            if (memberType != null && memberType._IsReferenceType())
            {
                byte[] nullableFlag = GetNullableAttributeValue(member, false) ??
                                      GetNullableAttributeValue(member.DeclaringType, true);

                return nullableFlag != null && nullableFlag.Length > 0 && nullableFlag[0] == 2;
            }

            return false;
        }

        public static bool IsReferenceForcedNullable(this ParameterInfo member)
        {
            // non reference types will be converted to Nullable<> if they are nullable.
            // C# 8.0 nullable flag just changes the way reference types are handled. If enabled, then they are
            // not implicitly nullable anymore!
            Type memberType = member.ParameterType;
            if (member.ParameterType._IsReferenceType())
            {
                byte[] nullableFlag = GetNullableAttributeValue(member, false) ??
                                      GetNullableAttributeValue(member.Member, true) ??
                                      GetNullableAttributeValue(member.Member.DeclaringType, true);

                return nullableFlag != null && nullableFlag.Length > 0 && nullableFlag[0] == 2;
            }

            return false;
        }

        internal static bool _IsGenericType(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsGenericType;
#else
            return t.IsGenericType;
#endif
        }

        internal static bool _IsGenericTypeDefinition(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsGenericTypeDefinition;
#else
            return t.IsGenericTypeDefinition;
#endif
        }
        internal static Type _BaseType(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().BaseType;
#else
            return t.BaseType;
#endif
        }
        internal static bool _IsEnum(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsEnum;
#else
            return t.IsEnum;
#endif
        }
        internal static bool _IsClass(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsClass;
#else
            return t.IsClass;
#endif
        }

        internal static bool _IsAssignableFrom(this Type t, Type t2)
        {
#if NETCORE
            return t.GetTypeInfo().IsAssignableFrom(t2);
#else
            return t.IsAssignableFrom(t2);
#endif
        }

        internal static bool _IsAbstract(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsAbstract;
#else
            return t.IsAbstract;
#endif
        }

        internal static bool _IsInterface(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsInterface;
#else
            return t.IsInterface;
#endif
        }

        internal static bool _IsArray(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().IsArray;
#else
            return t.IsArray;
#endif
        }

        internal static bool _IsReferenceType(this Type t)
        {
            return t._IsClass() || t._IsInterface() || t._IsArray() || typeof(string)._IsAssignableFrom(t);
        }

        internal static IEnumerable<Type> _GetInterfaces(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().GetInterfaces();
#else
            return t.GetInterfaces();
#endif
        }
        internal static FieldInfo[] _GetFields(this Type t, BindingFlags flags)
        {
#if NETCORE
            return t.GetTypeInfo().GetFields(flags);
#else
            return t.GetFields(flags);
#endif
        }

        internal static FieldInfo[] _GetFields(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().GetFields();
#else
            return t.GetFields();
#endif
        }

        internal static FieldInfo _GetField(this Type t, string name)
        {
#if NETCORE
            return t.GetTypeInfo().GetField(name, MembersFlags);
#else
            return t.GetField(name, MembersFlags);
#endif
        }

        internal static ConstructorInfo[] _GetConstructors(this Type t, BindingFlags flags)
        {
#if NETCORE
            return t.GetTypeInfo().GetConstructors(flags);
#else
            return t.GetConstructors(flags);
#endif
        }


        internal static PropertyInfo[] _GetProperties(this Type t, BindingFlags flags)
        {
#if NETCORE
            return t.GetTypeInfo().GetProperties(flags);
#else
            return t.GetProperties(flags);
#endif
        }

        internal static MethodInfo[] _GetMethods(this Type t, BindingFlags flags)
        {
#if NETCORE
            return t.GetTypeInfo().GetMethods(flags);
#else
            return t.GetMethods(flags);
#endif
        }

        internal static MethodInfo _GetMethod(this Type t, string name)
        {
#if NETCORE
            return t.GetTypeInfo().GetMethod(name, MembersFlags);
#else
            return t.GetMethod(name, MembersFlags);
#endif
        }
        internal static Type[] _GetGenericArguments(this Type t)
        {
#if NETCORE
            return t.GetTypeInfo().GetGenericArguments();
#else
            return t.GetGenericArguments();
#endif
        }

        internal static PropertyInfo _GetProperty(this Type t, string name)
        {
#if NETCORE
            return t.GetTypeInfo().GetProperty(name, MembersFlags);
#else
            return t.GetProperty(name, MembersFlags | BindingFlags.GetProperty | BindingFlags.SetProperty);
#endif
        }


        /// <summary>
        ///     Binding flags for searching all members
        /// </summary>
        public const BindingFlags MembersFlags = PublicMembersFlags | BindingFlags.NonPublic;

        /// <summary>
        ///     Binding flags for searching all members
        /// </summary>
        public const BindingFlags PublicMembersFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

#region Inheritance flatten
        /// <summary>
        /// Simple comparer to detect overridden methods
        /// </summary>
        private class MethodEqComparer : IEqualityComparer<MethodInfo>
        {
            public static readonly MethodEqComparer Instance = new MethodEqComparer();

            public bool Equals(MethodInfo x, MethodInfo y)
            {
                if (x == null && y == null) return true;
                if (x == null && y != null) return false;
                if (y == null && x != null) return false;

                if (x.Name != y.Name) return false;
                var xparms = x.GetParameters();
                var yparms = y.GetParameters();
                if (xparms.Length != yparms.Length) return false;
                for (int i = 0; i < xparms.Length; i++)
                {
                    if (xparms[i].Name != yparms[i].Name) return false;
                }
                return true;
            }

            public int GetHashCode(MethodInfo obj)
            {
                unchecked
                {
                    int hashCode = obj.Name.GetHashCode();
                    var parms = obj.GetParameters();
                    hashCode = (hashCode * 397) ^ parms.Length;
                    foreach (var pi in parms)
                    {
                        hashCode = (hashCode * 397) ^ pi.Name.GetHashCode();
                    }
                    return hashCode;
                }
            }
        }



        private class NameEqComparer<T> : IEqualityComparer<T> where T : MemberInfo
        {
            public static readonly NameEqComparer<T> Instance = new NameEqComparer<T>();

            public bool Equals(T x, T y)
            {
                if (x == null && y == null) return true;
                if (x == null && y != null) return false;
                if (y == null && x != null) return false;

                if (x.Name != y.Name) return false;
                return true;
            }

            public int GetHashCode(T obj) { return obj.Name.GetHashCode(); }
        }

        private static IEqualityComparer<T> GetInheritanceEqComparer<T>() where T : MemberInfo
        {
            if (typeof(T) == typeof(MethodInfo)) return (IEqualityComparer<T>)MethodEqComparer.Instance;
            return NameEqComparer<T>.Instance;
        }

        internal static T[] GetInheritedMembers<T>(this Type type, Func<Type, T[]> membersGetter, Type limiter)
            where T : MemberInfo
        {
            var members = new HashSet<T>(GetInheritanceEqComparer<T>());

            var considered = new List<Type>();
            var queue = new Queue<Type>();
            considered.Add(type);
            queue.Enqueue(type);
            bool limitReached = false;
            while (queue.Count > 0)
            {
                var subType = queue.Dequeue();

                foreach (var subInterface in subType._GetInterfaces())
                {
                    if (considered.Contains(subInterface)) continue;

                    considered.Add(subInterface);
                    queue.Enqueue(subInterface);
                }

                if (!limitReached)
                {
                    var bt = subType._BaseType();
                    if (bt != null)
                    {
                        if (bt == limiter)
                        {
                            limitReached = true;
                        }
                        else
                        {
                            if (considered.Contains(bt)) continue;

                            considered.Add(bt);
                            queue.Enqueue(bt);
                        }
                    }
                }

                var typeMembers = membersGetter(subType);
                foreach (var newMember in typeMembers)
                {
                    if (!members.Contains(newMember)) members.Add(newMember);
                }
            }
            return members.ToArray();
        }

        internal static IEnumerable<T> GetExportingMembers<T>(this TypeBlueprint t,
            bool flattenHierarchy,
            Func<Type, BindingFlags, T[]> membersGetter,
            Type limiter,
            bool publicOnly = false)
            where T : MemberInfo
        {
            var declaredFlags = publicOnly ? PublicMembersFlags : MembersFlags;

            T[] baseSet = null;
            baseSet = flattenHierarchy ?
                GetInheritedMembers(t.Type, x => membersGetter(x, declaredFlags), limiter)
                : membersGetter(t.Type, declaredFlags);

            var allMembers = baseSet.Where(x => (x.GetCustomAttribute<CompilerGeneratedAttribute>() == null) && !t.IsIgnored(x)).OfType<T>();
            return allMembers.ToArray();
        }

        internal static IEnumerable<T> GetExportingMembers<T>(this TypeBlueprint t,
            Func<Type, BindingFlags, T[]> membersGetter,
            bool publicOnly = false)
            where T : MemberInfo
        {
            var declaredFlags = publicOnly ? PublicMembersFlags : MembersFlags;
            var flattenHierarchy = t.TypeAttribute.FlattenHierarchy;
            var limiter = t.TypeAttribute.FlattenLimiter;

            T[] baseSet = null;
            baseSet = flattenHierarchy ?
                GetInheritedMembers(t.Type, x => membersGetter(x, declaredFlags), limiter)
                : membersGetter(t.Type, declaredFlags);

            var allMembers = baseSet.Where(x => (x.GetCustomAttribute<CompilerGeneratedAttribute>() == null) && !t.IsIgnored(x)).OfType<T>();
            return allMembers.ToArray();
        }

#endregion

#region IsStatic

        /// <summary>
        ///     Determines is type is static
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is static. False otherwise</returns>
        public static bool IsStatic(this Type t)
        {
#if NETCORE
            return (t.GetTypeInfo().IsAbstract && t.GetTypeInfo().IsSealed);
#else
            return (t.IsAbstract && t.IsSealed);
#endif
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

#endregion

#region Type distinguishing

        /// <summary>
        ///     Determines is type derived from Nullable or not
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is nullable value type. False otherwise</returns>
        public static bool IsNullable(this Type t)
        {
            return (t._IsGenericType() && (t.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Determines if type is one of System.Tuple types set
        /// </summary>
        /// <param name="t">Type to check</param>
        /// <returns>True when type is tuple, false otherwise</returns>
        public static bool IsTuple(this Type t)
        {
            if (!t._IsGenericType()) return false;
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
            if (gen.FullName != null && gen.FullName.StartsWith("System.ValueTuple`")) return true;

            return false;
        }

        /// <summary>
        ///     Determines if type is Dictionary-like
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True if type is derived from dictionary type</returns>
        public static bool IsDictionary(this Type t)
        {
            if (t._IsGenericType())
            {
                var tg = t.GetGenericTypeDefinition();

                if (typeof(IDictionary<,>)._IsAssignableFrom(tg)) return true;
                if (typeof(IReadOnlyDictionary<,>)._IsAssignableFrom(tg)) return true;
                if (typeof(Dictionary<,>)._IsAssignableFrom(tg)) return true;
                if (typeof(IDictionary)._IsAssignableFrom(t)) return true;
            }
            else
            {
                if (typeof(IDictionary)._IsAssignableFrom(t)) return true;
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
            if (typeof(IEnumerable)._IsAssignableFrom(t)) return true;
            if (t._IsGenericType())
            {
                var tg = t.GetGenericTypeDefinition();
                if (typeof(IEnumerable<>)._IsAssignableFrom(tg)) return true;
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
            if (t._IsGenericType() && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return false;
            var interfaces = t._GetInterfaces();
            var containsEnumerable = interfaces.Contains(typeof(IEnumerable));
            var containsGenericEnumerable =
                interfaces.Any(c => c._IsGenericType() && c.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return containsEnumerable && !containsGenericEnumerable;
        }

        /// <summary>
        ///     Determines if supplied type is delegate type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>True, if supplied type is delegate, false otherwise</returns>
        public static bool IsDelegate(this Type t)
        {
            if (t._BaseType() == null) return false;
            return typeof(MulticastDelegate)._IsAssignableFrom(t._BaseType());
        }

#endregion

#region Modifiers

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
        ///     Returns access modifier for specified method
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Access modifier string</returns>
        public static bool IsAsync(this MethodInfo methodInfo)
        {
            var ret = methodInfo.ReturnType;
            return typeof(Task)._IsAssignableFrom(ret);
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

#endregion

#region Utility methods

        /// <summary>
        ///     Retrieves first type argument of type
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>First type argument</returns>
        public static Type GetArg(this Type t)
        {
            return t._GetGenericArguments()[0];
        }

        /// <summary>
        ///     Removes generics postfix (all text after '`') from typename
        /// </summary>
        /// <param name="t">Type</param>
        /// <returns>Clean, genericless name</returns>
        internal static string CleanGenericName(this Type t)
        {
            if (t._IsGenericType())
            {
                var name = t.Name;
                var qidx = name.IndexOf('`');
                return name.Substring(0, qidx);
            }
            return t.Name;
        }

        internal static RtTypeName[] SerializeGenericArguments(this Type t)
        {
            if (!t._IsGenericTypeDefinition()) return new RtTypeName[0];
            if (t._IsGenericTypeDefinition())
            {
                // arranged generic attribute means that generic type is replaced with real one
                var args = t._GetGenericArguments().Where(g => g.GetCustomAttribute<TsGenericAttribute>() == null);
                var argsStr = args.Select(c => new RtSimpleTypeName(c.Name)).Cast<RtTypeName>().ToArray();
                return argsStr;
            }
            return new RtTypeName[0];
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

#endregion

        private static byte[] GetNullableAttributeValue(this ICustomAttributeProvider member, bool fromParent = false)
        {
            // need to retrieve all attributes and find by class name.
            // see: http://code.fitness/post/2019/02/nullableattribute.html
            // https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-metadata.md
            foreach(var customAttribute in member.GetCustomAttributes(true))
            {
                if (customAttribute is Attribute nullableAttribute) 
                {
                    bool isNullableAttribute = string.Equals(
                        customAttribute.GetType().FullName, 
                        "System.Runtime.CompilerServices.NullableAttribute",
                        StringComparison.InvariantCulture
                    ); 

                    bool isNullableContextAttribute = fromParent && !isNullableAttribute && string.Equals(
                       customAttribute.GetType().FullName, 
                       "System.Runtime.CompilerServices.NullableContextAttribute",
                       StringComparison.InvariantCulture
                    );

                    if (!isNullableAttribute && !isNullableContextAttribute)
                    {
                        continue;
                    }

                    FieldInfo flagField = isNullableAttribute
                        ? nullableAttribute.GetType().GetRuntimeField("NullableFlags")
                        : nullableAttribute.GetType().GetRuntimeField("Flag");
                        
                    object flagsData = flagField?.GetValue(nullableAttribute);
                    byte[] flags = flagsData is byte[] flagsDataBytes ? flagsDataBytes : null;
                    flags = flags is null && flagsData is byte flagsDataSingleByte ?
                        new byte[] {flagsDataSingleByte} : flags;

                    return flags != null && flags.Length > 0 ? flags : null;
                }
            }

            return null;
        }
    }
}